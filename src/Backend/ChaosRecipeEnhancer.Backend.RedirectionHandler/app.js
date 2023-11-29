export const handler = async (event, context) => {
    /*
     * This function handles the redirection after successful authentication in an OAuth flow.
     * It validates 'code' and 'state' from the query string parameters and redirects the user
     * to a custom URI scheme with these parameters.
     */

    // Extract 'code' and 'state' from query string parameters
    const queryStringParameters = event.queryStringParameters;
    if (!queryStringParameters || !queryStringParameters.code || !queryStringParameters.state) {
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Missing required parameters 'code' and 'state'." })
        };
    }

    const { code, state } = queryStringParameters;

    // Validate 'code' (codeChallenge) and 'state'
    // Assuming codeChallenge is a Base64 URL encoded SHA256 hash, and state is a Base64 URL encoded string
    if (!isValidBase64UrlEncoded(code) || !isValidBase64UrlEncoded(state)) {
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Invalid 'code' or 'state' format." })
        };
    }

    // Construct the redirect URL
    const params = new URLSearchParams('');
    params.append('code', code);
    params.append('state', state);
    const location = "chaosrecipe://auth?" + params.toString();

    // Generate HTTP redirect response
    const response = {
        statusCode: 302,
        headers: { location }
    };

    return response;
};

function isValidBase64UrlEncoded(str) {
    // Regex to check if string is Base64 URL encoded
    const base64UrlRegex = /^[A-Za-z0-9_-]+$/;
    return base64UrlRegex.test(str);
}