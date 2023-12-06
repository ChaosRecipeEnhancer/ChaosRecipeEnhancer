# CRE Backend

This is backend for the the Chaos Recipe Enhancer. The backend is responsible for handling a large portion of the OAuth flow between the CRE desktop application and GGG's services.

## Architecture Diagram

![Architecture Diagram](./DocumentationAssets/CRE-Architecture.drawio.png)

As the architecture diagram outlines, all back-end services as hosted on AWS (we leverage a few services across the platform).

And yes, I do pay for this stuff out of pocket. 

## Auth Flow Sequence

![Auth Flow Sequence Diagram](./DocumentationAssets/CRE-Auth-Sequence.png)

## Acknowledgements

MrTin (QT-Dev) on Discord / GitHub - author of the Exile Diary Reborn application.
He helped me a ton setting up the OAuth flow. Thanks!

## Contributing

I am open to contributions, but I'd like to keep the back-end codebase relatively small and simple.

If you have any ideas for features, please reach out to me on Discord or open an issue on GitHub.