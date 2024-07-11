# Why?

We manually include the WPF Toolkit in our project because it is not available as a NuGet package _without_ the inclusion of unecessary .dll's.

This shaves quite a few MB from our final package size, which is definitely something I want to be mindful of.

(For my own record-keeping) Most recently updateded this package to [v4.6.1](https://github.com/xceedsoftware/wpftoolkit/releases/tag/4.6.1)
