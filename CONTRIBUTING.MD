# How to contribute

There are many ways for you to contribute to ODataConnected service The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. 

## 1. Discussion

You can participate into discussions and ask questions about OData .NET at our [Github issues](https://github.com/OData/ODataConnectedService/issues).

## 2. Bug reports

When reporting a bug at the issue tracker, fill the relevant issue template. The issue related to other projects should not be reported in OData Connected Service issue tracker, but be reported to other projects' issue tracker.

## 3. Pull request for code and document contribution

**Pull request is the only way we accept code and document contribution.** Pull request of document, features and bug fixes are both welcomed. Refer to this [link](https://help.github.com/articles/using-pull-requests/) to learn details about pull request. Before you send a pull request to us, you need to make sure you've followed the steps listed below.

### Pick an issue to work on

You should either create or pick an issue on the [issue tracker](https://github.com/OData/ODataConnectedService/issues) before you work on the pull request. After the OData team has reviewed this issue and change its label to "accepting pull request", you can work on the code change.

### Prepare Tools

[Atom](https://atom.io/) with package [atom-beautify](https://atom.io/packages/atom-beautify) and [markdown-toc](https://atom.io/packages/markdown-toc) is recommended to edit the document. [MarkdownPad](http://www.markdownpad.com/) can also be used to edit the document.\
Visual Studio 2017 or 2019 are the recommended tools for code contribution.

### Steps to create a pull request

These are the recommended steps to create a pull request:

1. Create a forked repository of [(https://github.com/OData/ODataConnectedService.git](https://github.com/OData/ODataConnectedService.git)
2. Clone the forked repository into your local environment
3. Add a git remote to upstream for local repository with command _git remote add upstream (https://github.com/OData/ODataConnectedService.git]((https://github.com/OData/ODataConnectedService.git)_
4. Make code changes and add test cases
5. Test the changed code locally to make sure they pass
6. Commit changed code to local repository with clear message
7. Rebase the code to upstream via command _git pull --rebase upstream master_ and resolve conflicts if there is any then continue rebase via command _git pull --rebase continue_
8. Push local commit to the forked repository
9. Create pull request from forked repository Web console via comparing with upstream and add the ready for review label.
10. Complete a Contributor License Agreement (CLA), refer below section for more details.
11. Pull request will be reviewed by Microsoft OData team
12. Address comments and revise code if necessary
13. Commit the changes to local repository or amend existing commit via command _git commit --amend_
14. Rebase the code with upstream again via command _git pull --rebase upstream master_ and resolve conflicts if there is any then continue rebase via command _git pull --rebase continue_
15. Test the changed code again
16. Push changes to the forked repository and use _--force_ option if existing commit is amended
17. Microsoft OData team will merge the pull request into upstream

### Complete a Contribution License Agreement (CLA)

You will need to complete a Contributor License Agreement (CLA). Briefly, this agreement testifies that you are granting us permission to use the submitted change according to the terms of the project's license, and that the work being submitted is under appropriate copyright.

Please submit a Contributor License Agreement (CLA) before submitting a pull request. Download the agreement ([Microsoft Contribution License Agreement.pdf]("https://github.com/odata/odatacpp/wiki/files/Microsoft Contribution License Agreement.pdf")), sign, scan, and email it back to [cla@microsoft.com](mailto:cla@microsoft.com). Be sure to include your Github user name along with the agreement. Only after we have received the signed CLA, we'll review the pull request that you send. You only need to do this once for contributing to any Microsoft open source projects.