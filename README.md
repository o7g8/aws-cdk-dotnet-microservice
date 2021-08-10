# .NET Development on AWS Workshop

The repo contains submodules therefore clone it with:

```powershell
git clone --recurse-submodules <url>
```

Install [Hugo](http://gohugo.io) and serve the workshop content as described in [./workshop](./workshop).

## Useful CDK commands

- `dotnet build src` compile this app

- `cdk deploy`       deploy this stack to your default AWS account/region

- `cdk diff`         compare deployed stack with current state

- `cdk synth`        emits the synthesized CloudFormation template
