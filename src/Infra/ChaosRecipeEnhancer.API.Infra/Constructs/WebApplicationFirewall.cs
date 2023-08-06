using System.Collections.Generic;
using Amazon.CDK.AWS.WAFv2;
using Constructs;

namespace ChaosRecipeEnhancer.API.Infra.Constructs;

public class WebApplicationFirewallProps
{
    public string GatewayArn { get; set; }
}

public class WebApplicationFirewall : Construct
{
    private CfnWebACL WebAcl { get; }

    public WebApplicationFirewall(Construct scope, string id, WebApplicationFirewallProps props) : base(scope, id)
    {
        WebAcl = new CfnWebACL(
            this, 
            "WebApplicationFirewall",
            new CfnWebACLProps
            {
                Name = "CRE-WebACL",
                Description = "WebACL for CRE API Gateway",
                Scope = "REGIONAL",
                
                DefaultAction = new CfnWebACL.DefaultActionProperty
                {
                    Block = new CfnWebACL.BlockActionProperty
                    {
                        CustomResponse = new CfnWebACL.CustomResponseProperty
                        {
                            ResponseCode = 403
                        }
                    }
                },
                VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                {
                    CloudWatchMetricsEnabled = true,
                    MetricName = $"{id}-WebAclMetrics",
                    SampledRequestsEnabled = true
                },
                Rules = new CfnWebACL.RuleProperty[] {
                    new()
                    {
                        Name = "AWS-AWSManagedRulesCommonRuleSet",
                        Priority = 1,
                        OverrideAction = new CfnWebACL.OverrideActionProperty
                        {
                            None = new Dictionary<string, object> { }
                        },
                        Statement = new CfnWebACL.StatementProperty
                        {
                            ManagedRuleGroupStatement = new CfnWebACL.ManagedRuleGroupStatementProperty
                            {
                                Name = "AWSManagedRulesCommonRuleSet",
                                VendorName = "AWS"
                            }
                        },
                        VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                        {
                            CloudWatchMetricsEnabled = true,
                            MetricName = "awsCommonRules",
                            SampledRequestsEnabled = true
                        }
                    },
                    new()
                    {
                        Name = "AWS-AWSManagedRulesAnonymousIPRuleSet",
                        Priority = 2,
                        OverrideAction = new CfnWebACL.OverrideActionProperty
                        {
                            None = new Dictionary<string, object> { }
                        },
                        Statement = new CfnWebACL.StatementProperty
                        { 
                            ManagedRuleGroupStatement = new CfnWebACL.ManagedRuleGroupStatementProperty
                            {
                                Name = "AWSManagedRulesAnonymousIpList",
                                VendorName = "AWS"
                            },
                        },
                        VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                        {
                            CloudWatchMetricsEnabled = true,
                            MetricName = "awsAnonymousIp",
                            SampledRequestsEnabled = true
                        }
                    },
                    new()
                    {
                        Name = "AWS-AWSManagedRulesIPReputationRuleSet",
                        Priority = 3,
                        OverrideAction = new CfnWebACL.OverrideActionProperty
                        {
                            None = new Dictionary<string, object> { }
                        },
                        Statement = new CfnWebACL.StatementProperty
                        {
                            ManagedRuleGroupStatement = new CfnWebACL.ManagedRuleGroupStatementProperty
                            {
                                Name = "AWSManagedRulesAmazonIpReputationList",
                                VendorName = "AWS"
                            },
                        },
                        VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                        {
                            CloudWatchMetricsEnabled = true,
                            MetricName = "awsIpReputation",
                            SampledRequestsEnabled = true
                        }
                    },
                    new()
                    {
                        Name = "AWS-AWSManagedRulesKnownBadInputRuleSet",
                        Priority = 4,
                        OverrideAction = new CfnWebACL.OverrideActionProperty
                        {
                            None = new Dictionary<string, object> { }
                        },
                        Statement = new CfnWebACL.StatementProperty
                        {
                            ManagedRuleGroupStatement = new CfnWebACL.ManagedRuleGroupStatementProperty
                            {
                                Name = "AWSManagedRulesKnownBadInputsRuleSet",
                                VendorName = "AWS"
                            },
                        },
                        VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                        {
                            CloudWatchMetricsEnabled = true,
                            MetricName = "awsKnownBadInputs",
                            SampledRequestsEnabled = true
                        }
                    },
                }
            }
        );

        _ = new CfnWebACLAssociation(
            this,
            $"{id}-WebACL-Association",
            new CfnWebACLAssociationProps
            {
                ResourceArn = props.GatewayArn,
                WebAclArn = WebAcl.AttrArn
            }
        );
    }
}