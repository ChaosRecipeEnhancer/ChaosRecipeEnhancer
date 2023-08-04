using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra;

internal class GatewayProps
{
	public string GatewayName { get; set; } = default!;
	public string ApiVersion { get; set; } = default!;
	public string IdPrefix { get; set; } = default!;
	public Function ApiFunction { get; set; } = default!;
	public Environment BaseEnvironmentVariables { get; set; } = default!;
}

public class Gateway : Construct
{
	internal LambdaRestApi RestApiGateway
	{
		get; set;
	}

	internal Gateway(Construct scope, string id, GatewayProps props) : base(scope, id)
	{
		var apiGatewayLogGroup = new LogGroup(this, $"{props.IdPrefix}ApiGatewayLogGroup", new LogGroupProps()
		{
			LogGroupName = $"/aws/apigateway/{props.GatewayName}",
			Retention = RetentionDays.ONE_DAY,
			RemovalPolicy = RemovalPolicy.DESTROY
		});

		RestApiGateway = new LambdaRestApi(
			this,
			$"{props.IdPrefix}RestApiGateway",
			new LambdaRestApiProps()
			{
				RestApiName = props.GatewayName,
				Handler = props.ApiFunction,
				IntegrationOptions = new LambdaIntegrationOptions
				{
					AllowTestInvoke = false,
				},
				Description = $"Chaos Recipe Enhancer API Gateway {props.ApiVersion}",
				EndpointTypes = new EndpointType[] { EndpointType.REGIONAL },
				DeployOptions = new StageOptions()
				{
					StageName = props.ApiVersion,

					CachingEnabled = false,
					ThrottlingRateLimit = 25,
					ThrottlingBurstLimit = 10,

					Variables = new Dictionary<string, string>() {
						{ "Environment", props.BaseEnvironmentVariables.Name },
						{ "ApiVersion", props.ApiVersion }
					},

					LoggingLevel = MethodLoggingLevel.INFO,
					TracingEnabled = true,
					AccessLogFormat = AccessLogFormat.Clf(),
					AccessLogDestination = new LogGroupLogDestination(apiGatewayLogGroup),
				},

				DomainName = new DomainNameOptions()
				{
					DomainName = props.BaseEnvironmentVariables.ApiGatewayUrl,
					Certificate = Certificate.FromCertificateArn(
						this,
						"CRE-API-Certificate",
						props.BaseEnvironmentVariables.CertificateArn
					),
					SecurityPolicy = SecurityPolicy.TLS_1_2,
					EndpointType = EndpointType.REGIONAL,
				},

				CloudWatchRole = true,
				Proxy = false
			}
		);

		var defaultNoneAuthorizationOptions = new MethodOptions()
		{
			AuthorizationType = AuthorizationType.NONE,
			Authorizer = null,
			AuthorizationScopes = new string[] { }
		};

		var defaultAllowAllCorsOptions = new CorsOptions()
		{
			AllowOrigins = Cors.ALL_ORIGINS,
			AllowMethods = Cors.ALL_METHODS,
			AllowHeaders = Cors.DEFAULT_HEADERS,
			MaxAge = Duration.Seconds(3600)
		};

		var rootResource = RestApiGateway.Root;

		rootResource.AddProxy(new ProxyResourceOptions()
		{
			AnyMethod = true,
			DefaultIntegration = new LambdaIntegration(props.ApiFunction, new LambdaIntegrationOptions { }),
			DefaultCorsPreflightOptions = defaultAllowAllCorsOptions
		});

		var swaggerResource = rootResource.AddResource(
			"swagger",
			new ResourceOptions()
			{
				DefaultMethodOptions = defaultNoneAuthorizationOptions,
				DefaultCorsPreflightOptions = defaultAllowAllCorsOptions
			}
		);

		var swaggerMethod = swaggerResource.AddMethod(
			"GET",
			new LambdaIntegration(props.ApiFunction)
		);

		var swaggerProxyResource = swaggerResource.AddProxy(
			new ProxyResourceOptions()
			{
				DefaultMethodOptions = defaultNoneAuthorizationOptions,
				DefaultIntegration = new LambdaIntegration(props.ApiFunction,
					new LambdaIntegrationOptions
					{
						AllowTestInvoke = false,
					}
				)
			}
		);
	}
}
