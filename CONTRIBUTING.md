# Contributing to the Chaos Recipe Enhancer (EnhancePoEApp) Repository

We appreciate any help in the project we can get. Before doing so, however, we have some guidelines we'd like for you to follow:

 - [Submission Guidelines](#submit)
 - [Required extensions](#extensions)
 - [Coding Guidelines](#coding)

## Submission Guidelines

Before you submit your Pull Request (PR) consider the following guidelines:

1. Search [GitHub](https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/pulls) for an open or closed PR
  that relates to your submission before creating a new one.
1. Be sure that an issue describes the problem you're fixing (If not, please [create an issue on our board](https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/issues/new/choose)).
1. Make your changes in a new git branch:

   For new features and enhancements:
     ```shell
     git checkout -b feature/{{ your github username }}/{{ very short description of feature }} develop
     ```

   For new bug fixes:
     ```shell
     git checkout -b bugfix/{{ your github username }}/{{ very short description of bugfix }} develop
     ```

   Example:
     ```shell
     git checkout -b bugfix/himariolopez/crucible-stash-items-not-loading
     ```

1. Commit your changes using a descriptive commit message.

     ```shell
     git commit -a
     ```

    Note: the optional commit `-a` command line option will automatically "add" and "rm" edited files.

1. Push your branch to GitHub:

    ```shell
    git push origin my-fix-branch
    ```

1. In GitHub, send a pull request to our `develop` branch, or `EnhancePoEApp:develop` if you have forked the repo.

## Coding Guidelines

TODO: We will update this as needed.

1. Check spelling on all code submitted for review.
1. Avoid adding new 3rd party dependencies (NuGet packages) unless discussed with Mario (Lead Developer)