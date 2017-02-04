# How to contribute

[日本語のコントリビューションガイドはこちら！](CONTRIBUTING-ja.md)

Third-party contributions are essential for the future development of Xamarin.Forms.GoogleMaps.
We want to keep it as easy as possible to contribute changes that get things working
in your environment. There are a few guidelines that we need contributors to follow
so that we can keep on top of things.

## Getting Started

* Make sure you have a [GitHub account](https://github.com/signup/free).
* Create an Issue for your problem, assuming one does not already exist.
  * Clearly describe the issue including steps to reproduce, stacktrace and environments when it is a bug.
  * We have a issue tamplate, you can use a part of it.
* Fork the repository on GitHub.
* After the clone your repogitory to local, you should set ``git config user.name`` and `` git config user.email your@ema.il`` . Especially you **MUST** set ``user.email`` as same as your GitHub account's e-mail.

## Making Changes

* Create a topic branch from where you want to base your work.
  * This is usually the master branch.
  * To quickly create a topic branch based on master; `git branch
    issue_999 master` then checkout the new branch with `git
    checkout issue_999`. Please avoid working directly on the
    `master` branch.
* Make commits of logical units. **Do not contains unrelated file changes(e.g. code formatting).**
* Check for unnecessary whitespace with `git diff --check` before committing.
* Make sure your commit messages are in the proper format.

````
Essential commit message information here.

The first line summarizes the patch content.
The body paragraph describes the behavior without the patch,
why this is a problem, and how the patch fixes the problem when applied.
````

* Make sure you have added the necessary tests for your changes.
* Run _all_ the tests to assure nothing else was accidentally broken.

### Coding Style

We follow the style used by the [.NET Foundation](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md), with one primary exception:

> We use Allman style braces, where each brace begins on a new line. ~~A single line statement block can go without braces~~

We **always** need braces, you can not got without braces.

**Examples:**

```csharp
// 👎🏽 DO NOT USE
if (source == null) 
    throw new ArgumentNullException("source");

// 👍🏽 GOOD
if (source == null)
{
    throw new ArgumentNullException("source");
}
```

## Submitting Changes

* Push your changes to a topic branch in your fork of the repository.
* Submit a pull request to the Xamarin.Forms.GoogleMaps repository.

Make pull request guide line

* Write the summary of the changes in an easy-to-understand for the title.
* Reveal a usage or test code for your changes. We storongly recommend to add usage of new feature to sample apps.
* Include related issue number for then contents. (e.g. ref #199)
* If you are work in progress, you should start title with [WIP]. If you worked out, delete the [WIP] and please let us know.
  * If we changed the master before you completed the work, you should resolve conflicts.
  * We allow you send [WIP] pull request first what means issue reservation. But if you no longer active, we will close it.

# Thanks

This guide is based on [MMP/CONTRIBUTING.en.md · sn0w75/MMP](https://github.com/sn0w75/MMP/blob/master/CONTRIBUTING.en.md).