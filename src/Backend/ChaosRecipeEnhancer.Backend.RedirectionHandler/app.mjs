export const handler = async (event, context) => {
    /*
     * This function handles the redirection after successful authentication in an OAuth flow.
     * It validates 'code' and 'state' from the query string parameters and redirects the user
     * to a custom URI scheme with these parameters.
     */

    // Extract 'code' and 'state' from query string parameters
    const queryStringParameters = event.queryStringParameters;
    if (!queryStringParameters || !queryStringParameters.code || !queryStringParameters.state) {
        console.log(`Handler: Error! Missing required parameters 'code' and 'state'.`);
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Missing required parameters 'code' and 'state'." })
        };
    }

    const { code, state } = queryStringParameters;

    // Validate 'code' (codeChallenge)
    // Assuming codeChallenge is a Base64 URL encoded SHA256 hash
    if (!isValidBase64UrlEncoded(code)) {
        console.log(`Handler: Error! Invalid 'code' format.`);
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Invalid 'code' format." })
        };
    }

    return serveAuthSuccessfulPage(code, state);
};

function serveAuthSuccessfulPage(code, state) {
    const redirectUri = `chaosrecipe://auth?code=${encodeURIComponent(code)}&state=${encodeURIComponent(state)}`;
    const htmlContent = `
    <!DOCTYPE html>
    <html>
        <head>
            <title>Authentication Successful</title>
            <script type="text/javascript">
                function redirectToCreClient() {
                    window.location.href = '${redirectUri}';
                }
                window.onload = function() {
                    setTimeout(redirectToCreClient, 200); // Redirect after 3 seconds
                };
            </script>
        </head>
        <body>
            <h1>Authentication for Chaos Recipe Enhancer Successful!</h1>
            <p>This page is safe to close now.</p>
            <p>If the app does not open automatically, <a href="javascript:redirectToCreClient()">please click here</a>.</p>
        </body>
    </html>
    `;

    return {
        statusCode: 200,
        headers: { 'Content-Type': "text/html" },
        body: htmlContent
    };
}

function isValidBase64UrlEncoded(str) {
    // Decode URL encoded string
    let decodedStr;
    try {
        decodedStr = decodeURIComponent(str);
        console.log(`Utility - Info. Decoded URL encoded string: ${decodedStr}`);
    } catch (e) {
        console.log(`Utility - Info. Failed to decode URL encoded string: ${e}`);
        return false;
    }

    // Regex to check if string is Base64 URL encoded
    // This regular expression now allows for regular Base64 characters, and also '+', '/', and '='
    const base64UrlRegex = /^[A-Za-z0-9_\-+/=]+$/;
    return base64UrlRegex.test(decodedStr);
}
