# Branch protection

Direct pushes to the default branch (`master`/`main`) are prevented via GitHub branch
protection rules (these cannot be enforced by files in the repository).

Configure in **Settings → Branches → Branch protection rules** for `master` (and `main`
if it exists):

- [x] Require a pull request before merging
- [x] Require approvals (at least 1)
- [x] Require status checks to pass before merging
  - `CI / Build and test`
  - `Commit Lint / Lint commit messages`
  - `PR Title / Validate PR title`
- [x] Require branches to be up to date before merging
- [x] Do not allow bypassing the above settings
- [x] Restrict who can push to matching branches (no direct pushes)

The `CI` workflow runs the unit tests on every pull request and will block merging
until they pass.
