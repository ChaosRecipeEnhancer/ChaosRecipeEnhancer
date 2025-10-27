/* global fetch, btoa, atob */

import {
  SecretsManagerClient,
  GetSecretValueCommand,
} from "@aws-sdk/client-secrets-manager";

/**
 * This Lambda function retrieves an authentication token from the Path of Exile API.
 * It first retrieves the client secret from AWS Secrets Manager, then uses it along with the
 * provided code and code_verifier to obtain the token.
 */

/* This uses the AWS Secrets Manager to get the secret key for the GGGAUTH API. */
/* Docs: https://docs.aws.amazon.com/secretsmanager/latest/userguide/retrieving-secrets_lambda.html */
const getClientSecret = async () => {
  const client = new SecretsManagerClient({
    region: "us-east-1",
  });

  let response;

  try {
    const secretName = "CRE/Secrets/GGGAuth";
    response = await client.send(
      new GetSecretValueCommand({
        SecretId: secretName,
        VersionStage: "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified
      })
    );
  } catch (error) {
    // For a list of exceptions thrown, see
    // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
    throw error;
  }

  const secretsJson = response.SecretString;
  const parsedJson = JSON.parse(secretsJson); // Parse the entire JSON string
  const clientSecret = parsedJson.clientSecret; // Extract the clientSecret

  return clientSecret;
};

/**
 * Retrieves the authentication token from the Path of Exile API.
 * @param {Object} params - Parameters required for the token request.
 * @param {string} params.secretKey - The client secret key.
 * @param {string} params.code - The authorization code.
 * @param {string} params.codeVerifier - The code verifier string.
 * @param {string} [params.userAgent] - Optional User-Agent header to propagate.
 * @returns {Promise<Object>} The token response JSON.
 */
const getAuthToken = async ({
  secretKey,
  code,
  codeVerifier,
  userAgent,
}) => {
  const endpoint = "https://www.pathofexile.com/oauth/token";
  const headers = { "Content-Type": "application/x-www-form-urlencoded" };

  // Propagate User-Agent if provided (e.g., from incoming event.headers)
  if (userAgent) {
    headers["User-Agent"] = userAgent;
  }

  const urlencodedParams = new URLSearchParams();
  urlencodedParams.append("client_id", "chaosrecipeenhancer");
  urlencodedParams.append("client_secret", secretKey);
  urlencodedParams.append("grant_type", "authorization_code");
  urlencodedParams.append("code", code);

  // Prod Redirect Url
  urlencodedParams.append(
    "redirect_uri",
    "https://chaos-recipe.com/auth/success"
  );

  // Sandbox Redirect Url
  // urlencodedParams.append("redirect_uri", "https://sandbox.chaos-recipe.com/auth/success");

  urlencodedParams.append(
    "scope",
    "account:leagues account:stashes account:characters account:item_filter account:guild:stashes"
  );
  urlencodedParams.append("code_verifier", codeVerifier);

  // Log full request details before sending (for debugging)
  console.log(
    `getAuthToken --- Outgoing Headers: ${JSON.stringify(headers)}`
  );
  console.log(
    `getAuthToken --- Outgoing Body: ${urlencodedParams.toString()}`
  );

  const tokenResponse = await fetch(endpoint, {
    method: "POST",
    body: urlencodedParams,
    headers,
  });

  if (!tokenResponse.ok) {
    console.log(
      `getAuthToken --- Failed to retrieve token: ${tokenResponse.statusText}`
    );

    // Log the complete error response
    const errorResponse = await tokenResponse.text();
    console.log(`getAuthToken --- Error response: ${errorResponse}`);

    throw new Error(`Failed to retrieve token: ${tokenResponse.statusText}`);
  }

  // Debugging Issues
  // console.log(`getAuthToken --- tokenResponse:${JSON.stringify(tokenResponse)}`);

  return tokenResponse.json();
};

export const handler = async (event, context) => {
  try {
    // Log incoming headers for debugging propagation issues
    console.log(
      `handler --- Incoming Headers: ${JSON.stringify(event.headers)}`
    );

    const secretKey = await getClientSecret();
    const params = new URLSearchParams(atob(event.body));
    const code = params.get("code");
    const codeVerifier = params.get("code_verifier");

    if (!code || !codeVerifier) {
      console.log(
        `handler --- Error! Missing 'code' or 'code_verifier' in request.`
      );
      return {
        statusCode: 400,
        body: JSON.stringify({
          message: "Missing 'code' or 'code_verifier' in request.",
        }),
      };
    }

    // Extract User-Agent from incoming event.headers (case-insensitive fallback)
    const userAgent =
      event.headers["User-Agent"] ||
      event.headers["user-agent"] ||
        "OAuth,chaosrecipeenhancer/TokenRetrievalV2Backend,(contact: dev@chaos-recipe.com)"; // Default

    const token = await getAuthToken({
      secretKey,
      code,
      codeVerifier,
      userAgent,
    });

    return {
      statusCode: 200,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(token),
    };
  } catch (error) {
    console.log(`handler --- Error! ${error.message}`);
    return {
      statusCode: 500,
      body: JSON.stringify({ message: error.message }),
    };
  }
};