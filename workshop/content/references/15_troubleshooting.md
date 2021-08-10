---
title: "Troubleshooting"
date: 2021-08-10T20:45:40+02:00
draft: false
weight: 15
---

- If you fiddle with the name of the SSM Parameter, CDK may fail to deploy the changes, in this case you may want to redeploy the stack as:

  ```powershell
  cdk destroy
  cdk deploy
  ```
