var listOfCreClientVersionsToExclude = [
    // Add versions here (as strings) that are not allowed to authenticate
    // This can help us block older versions that may not be compatible
    // with the latest API changes or have security vulnerabilities

    // e.g. "3.24.1001"
];

const isClientVersionAllowed = (version) => {
    // Eventually we will enforce the version to be present in the state.
    // For now, we want to maintain backward compatibility with older versions.
    // I'm targetting... 3.25? 3.26? to enforce this.

    if (!version) return true;

    // versions before 3.24.200 will not include the version in the state
    //if (!version || /^\s*$/.test(version)) {
    //    console.log(`Handler: Error! Missing or invalid 'cre_client_id' in request.`);
    //    return false;
    //}

    // Check if the version is in the list of versions to exclude
    if (listOfCreClientVersionsToExclude.includes(version)) {
        console.log(`Handler: Error! 'cre_client_id' version is not allowed.`);
        return false;
    }

    return true;
};

export const handler = async (event, context) => {
    /*
     * This function handles the redirection after successful authentication in an OAuth flow.
     * It validates 'code' and 'state' from the query string parameters and redirects the user
     * to a custom URI scheme with these parameters.
     */

    // Extract 'code' and 'state' from query string parameters
    const queryStringParameters = event.queryStringParameters;
    if (!queryStringParameters ||
        !queryStringParameters.code ||
        !queryStringParameters.state
    ) {
        console.log(`Handler: Error! Missing required parameters 'code' and 'state'.`);
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Missing required parameters 'code' and 'state'." })
        };
    }

    let { code, state } = queryStringParameters;

    // Extract creClientVersion from the state parameter
    let creClientVersion = null;
    if (state) {
        const [originalState, clientVersion] = state.split("|");
        creClientVersion = clientVersion;
        state = originalState;
    }

    if (!isClientVersionAllowed(creClientVersion)) {
        console.log(`Handler: Error! Missing or invalid 'cre_client_id' in request.`);
        return serveUpdateRequiredPage();
    }

    console.log(`Handler: creClientVersion: ${creClientVersion}`);

    // Validate 'code' (codeChallenge)
    // Assuming codeChallenge is a Base64 URL encoded SHA256 hash
    if (!isValidBase64UrlEncoded(code)) {
        console.log(`Handler: Error! Invalid 'code' format.`);
        return {
            statusCode: 400,
            body: JSON.stringify({ message: "Invalid 'code' format." })
        };
    }

    if (!creClientVersion) {
        return serveAuthSuccessfulPageWithCreClientVersion(code, state, creClientVersion);
    } else {
        return serveAuthSuccessfulPage(code, state);
    }
};

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

function serveAuthSuccessfulPageWithCreClientVersion(code, state, creClientVersion) {
    const redirectUri = `chaosrecipe://auth?code=${encodeURIComponent(code)}&state=${encodeURIComponent(state)}&cre_client_version=${encodeURIComponent(creClientVersion)}`;
    const htmlContent = `
    <!DOCTYPE html>
    <html>
        <head>
            <title>Authentication Successful</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #1c1c1c;
                    color: #f0f0f0;
                    margin: 20px;
                }
                a {
                    color: #007bff;
                    text-decoration: none;
                }
                a:hover {
                    text-decoration: underline;
                }
            </style>
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
            <p>This page is safe to close now. It may take a few seconds (5-10 seconds) for your auth status to update in-app.</p>
            <p>If the app does not open and authenticate after a few moments, <a href="javascript:redirectToCreClient()">please click here</a>.</p>
        </body>
    </html>
    `;

    return {
        statusCode: 200,
        headers: { 'Content-Type': "text/html" },
        body: htmlContent
    };
}

function serveAuthSuccessfulPage(code, state) {
    const redirectUri = `chaosrecipe://auth?code=${encodeURIComponent(code)}&state=${encodeURIComponent(state)}}`;
    const htmlContent = `
    <!DOCTYPE html>
    <html>
        <head>
            <title>Authentication Successful</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #1c1c1c;
                    color: #f0f0f0;
                    margin: 20px;
                }
                a {
                    color: #007bff;
                    text-decoration: none;
                }
                a:hover {
                    text-decoration: underline;
                }
            </style>
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
            <p>This page is safe to close now. It may take a few seconds (5-10 seconds) for your auth status to update in-app.</p>
            <p>If the app does not open and authenticate after a few moments, <a href="javascript:redirectToCreClient()">please click here</a>.</p>
        </body>
    </html>
    `;

    return {
        statusCode: 200,
        headers: { 'Content-Type': "text/html" },
        body: htmlContent
    };
}

function serveUpdateRequiredPage() {
    const htmlContent = `
    <!DOCTYPE html>
    <html>
        <head>
            <title>Update Required</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #1c1c1c;
                    color: #f0f0f0;
                    margin: 20px;
                }
                a {
                    color: #007bff;
                    text-decoration: none;
                }
                a:hover {
                    text-decoration: underline;
                }
            </style>
        </head>
        <body>
            <h1>Chaos Recipe Enhancer Update Required</h1>
            <p>Your current version of Chaos Recipe Enhancer is outdated and requires an update to proceed with authentication.</p>
            <p>Please download the latest version of Chaos Recipe Enhancer from the official releases page:</p>
            <p><a href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/releases" target="_blank">https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/releases</a></p>
            <p>If you have any questions or need assistance, please visit our <a href="https://discord.gg/ryss9jnRkZ" target="_blank">Discord</a>.</p>
        </body>
    </html>
    `;

    return {
        statusCode: 426,
        headers: { 'Content-Type': "text/html" },
        body: htmlContent
    };
}