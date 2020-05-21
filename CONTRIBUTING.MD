# How to contribute

There are many ways for you to contribute to OData Connected service The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. 

## 1. Discussion

You can participate into discussions and ask questions about  OData Connected service at our [Github issues](https://github.com/OData/ODataConnectedService/issues).

## 2. Bug reports

When reporting a bug at the issue tracker, fill the relevant issue template. The issue related to other projects should not be reported in OData Connected Service issue tracker, but be reported to other projects' issue tracker.

## 3. Pull request for code and document contribution

**Pull request is the only way we accept code and document contribution.** Pull request of document, features and bug fixes are both welcomed. Refer to this [link](https://help.github.com/articles/using-pull-requests/) to learn details about pull request. Before you send a pull request to us, you need to make sure you've followed the steps listed below.

### Pick an issue to work on

You should either create or pick an issue on the [issue tracker](https://github.com/OData/ODataConnectedService/issues) before you work on the pull request. After the OData team has reviewed this issue and change its label to "accepting pull request", you can work on the code change.


### Steps to create a pull request

These are the recommended steps to create a pull request:

1. Create a forked repository of [ODataConnectedService](https://github.com/OData/ODataConnectedService.git)
2. Clone the forked repository into your local environment
3. Add a git remote to upstream for local repository with command `git remote add upstream https://github.com/OData/ODataConnectedService.git`
4. Create a branch for the new changes `git checkout -b {branch_name}` where branch_name uses the following naming conventions. fix/{issue-number}-title for bug fixes. feature/{issue-number}-title for feature branches.
5. Make code changes and add test cases
6. Test the changed code locally to make sure they pass
7. Commit changed code to local repository with clear message
8. Rebase the code to upstream via command `git pull --rebase upstream master` and resolve conflicts if there is any then continue rebase via command `git pull --rebase continue`
9. Push local commit to the forked repository
10. Create pull request from forked repository Web console via comparing with upstream and add the ready for review label.
11. Complete a Contributor License Agreement (CLA), refer below section for more details.
12. Pull request will be reviewed by Microsoft OData team
13. Address comments and revise code if necessary
13. Commit the changes to local repository or amend existing commit via command `git commit --amend`
15. Rebase the code with upstream again via command `git pull --rebase upstream master` and resolve conflicts if there is any then continue rebase via command `git pull --rebase continue`
16. Test the changed code again
17. Push changes to the forked repository and use `--force` option if existing commit is amended
18. Microsoft OData team will merge the pull request into upstream

### Complete a Contribution License Agreement (CLA)

You will need to sign a [Contributor License Agreement](https://cla.dotnetfoundation.org/) when submitting your pull request. To complete the Contributor License Agreement (CLA), you will need to follow the instructions provided by the CLA bot when you send the pull request. This needs to only be done once for any .NET Foundation OSS project.