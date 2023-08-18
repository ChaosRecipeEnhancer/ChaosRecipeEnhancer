using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.WAFv2;
using ChaosRecipeEnhancer.API.Infra.Helpers;
using Constructs;
using Environment = ChaosRecipeEnhancer.API.Infra.Config.Environment;

namespace ChaosRecipeEnhancer.API.Infra.Constructs;

public class GatewayProps
{
    public Function ApiFunction { get; init; } = default!;
    public string ApiGatewayUrl { get; set; } = default!;
    public string CertificateArn { get; set; } = default!;
    public string WebApplicationFirewallArn { get; set; } = default!;
}

public class Gateway : Construct
{
    public LambdaRestApi RestApiGateway { get; }
    public string GatewayArn { get; }
    private string? ApiVersion { get; }
    
    public Gateway(Construct scope, string id, GatewayProps props) : base(scope, id)
    {
        ApiVersion = InfraConfigHelper.GetApiVersion();
        
        var apiGatewayLogGroup = new LogGroup(
            this,
            $"RestApiGatewayLogGroup",
            new LogGroupProps
            {
                LogGroupName = $"/aws/apigateway/CRE-{id}",
                Retention = RetentionDays.ONE_DAY,
                RemovalPolicy = RemovalPolicy.DESTROY
            }
        );

        RestApiGateway = new LambdaRestApi(
            this,
            "RestApiGateway",
            new LambdaRestApiProps
            {
                RestApiName = "CRE-RestApiGateway",
                Handler = props.ApiFunction,
                IntegrationOptions = new LambdaIntegrationOptions { AllowTestInvoke = false, },
                Description = $"Chaos Recipe Enhancer API Gateway {ApiVersion}",
                EndpointTypes = new[] { EndpointType.REGIONAL },
                DeployOptions = new StageOptions
                {
                    StageName = ApiVersion,
                    CachingEnabled = false,
                    ThrottlingRateLimit = 25,
                    ThrottlingBurstLimit = 10,
                    Variables = new Dictionary<string, string>()
                    {
                        { "ApiVersion", ApiVersion }
                    },
                    LoggingLevel = MethodLoggingLevel.INFO,
                    TracingEnabled = true,
                    AccessLogFormat = AccessLogFormat.Clf(),
                    AccessLogDestination = new LogGroupLogDestination(apiGatewayLogGroup),
                },
                DomainName = new DomainNameOptions
                {
                    DomainName = props.ApiGatewayUrl,
                    Certificate = Certificate.FromCertificateArn(
                        this,
                        "CRE-API-Certificate",
                        props.CertificateArn
                    ),
                    SecurityPolicy = SecurityPolicy.TLS_1_2,
                    EndpointType = EndpointType.REGIONAL,
                },
                CloudWatchRole = true,
                Proxy = false
            }
        );

        var defaultNoneAuthorizationOptions = new MethodOptions
        {
            AuthorizationType = AuthorizationType.NONE,
            Authorizer = null,
            AuthorizationScopes = System.Array.Empty<string>()
        };

        var defaultAllowAllCorsOptions = new CorsOptions
        {
            AllowOrigins = Cors.ALL_ORIGINS,
            AllowMethods = Cors.ALL_METHODS,
            AllowHeaders = Cors.DEFAULT_HEADERS,
            MaxAge = Duration.Seconds(3600)
        };

        var rootResource = RestApiGateway.Root;

        rootResource.AddProxy(
            new ProxyResourceOptions
            {
                AnyMethod = true,
                DefaultIntegration = new LambdaIntegration(
                    props.ApiFunction,
                    new LambdaIntegrationOptions()
                ),
                DefaultCorsPreflightOptions = defaultAllowAllCorsOptions
            }
        );

        var swaggerResource = rootResource.AddResource(
            "swagger",
            new ResourceOptions
            {
                DefaultMethodOptions = defaultNoneAuthorizationOptions,
                DefaultCorsPreflightOptions = defaultAllowAllCorsOptions
            }
        );

        swaggerResource.AddMethod(
            "GET",
            new LambdaIntegration(props.ApiFunction)
        );

        swaggerResource.AddProxy(
            new ProxyResourceOptions
            {
                DefaultMethodOptions = defaultNoneAuthorizationOptions,
                DefaultIntegration = new LambdaIntegration(
                    props.ApiFunction,
                    new LambdaIntegrationOptions { AllowTestInvoke = false, }
                )
            }
        );

        GatewayArn = $"arn:aws:apigateway:{Stack.Of(this).Region}::/restapis/{RestApiGateway.RestApiId}/stages/{RestApiGateway.DeploymentStage.StageName}";
    }
}
