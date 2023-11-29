/* global fetch, btoa, atob */
/**
 * This Lambda function retrieves an authentication token from the Path of Exile API.
 * It first retrieves the client secret from AWS Secrets Manager, then uses it along with the
 * provided code and code_verifier to obtain the token.
 */

/**
 * Retrieves the client secret key from AWS Secrets Manager.
 * @returns {Promise<string>} The client secret key.
 */
const getSecretKey = async () => {
    const secretName = "CRE/Secrets/GGGAuth";
    const endpoint = `http://localhost:2773/secretsmanager/get?secretId=${secretName}`;
    const headers = { "X-Aws-Parameters-Secrets-Token": process.env.AWS_SESSION_TOKEN };

    const secretsResponse = await fetch(endpoint, { headers });

    if (!secretsResponse.ok) {
        throw new Error(`Failed to retrieve secret: ${secretsResponse.statusText}`);
    }

    const secretsJson = await secretsResponse.json();
    const { clientSecret } = JSON.parse(secretsJson.clientSecret);
    return clientSecret;
};

/**
 * Retrieves the authentication token from the Path of Exile API.
 * @param {Object} params - Parameters required for the token request.
 * @param {string} params.secretKey - The client secret key.
 * @param {string} params.code - The authorization code.
 * @param {string} params.code_verifier - The code verifier string.
 * @returns {Promise<Object>} The token response JSON.
 */
const getAuthToken = async ({ secretKey, code, code_verifier }) => {
    const endpoint = "https://www.pathofexile.com/oauth/token";
    const headers = { "Content-Type": "application/x-www-form-urlencoded" };

    const urlencodedParams = new URLSearchParams();
    urlencodedParams.append("client_id", "chaosrecipeenhancer");
    urlencodedParams.append("client_secret", secretKey);
    urlencodedParams.append("grant_type", "authorization_code");
    urlencodedParams.append("code", code);
    urlencodedParams.append("redirect_uri", "https://sandbox.chaos-recipe.com/auth/success");
    urlencodedParams.append("scope", "account:leagues account:stashes account:characters account:item_filter service:leagues service:psapi");
    urlencodedParams.append("code_verifier", code_verifier);

    const tokenResponse = await fetch(endpoint, { method: "POST", body: urlencodedParams, headers });

    if (!tokenResponse.ok) {
        throw new Error(`Failed to retrieve token: ${tokenResponse.statusText}`);
    }

    return tokenResponse.json();
};

export const handler = async (event, context) => {
    try {
        const secretKey = await getSecretKey();
        const params = new URLSearchParams(atob(event.body));
        const code = params.get('code');
        const code_verifier = params.get('code_verifier');

        if (!code || !code_verifier) {
            return {
                statusCode: 400,
                body: JSON.stringify({ message: "Missing 'code' or 'code_verifier' in request." })
            };
        }

        const token = await getAuthToken({ secretKey, code, code_verifier });

        return {
            statusCode: 200,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(token)
        };
    } catch (error) {
        return {
            statusCode: 500,
            body: JSON.stringify({ message: error.message })
        };
    }
};
