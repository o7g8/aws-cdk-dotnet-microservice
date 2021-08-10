---
title: "Micorservice: enable X-Ray"
date: 2021-08-10T22:00:12+02:00
draft: true
---

See <https://docs.aws.amazon.com/lambda/latest/dg/services-xray.html>

Do <https://docs.aws.amazon.com/lambda/latest/dg/csharp-tracing.html>:

- enable tracing;

- install the package and paste the code

- grant `lambda:GetAccountSettings` to the Lambda role.

Observe that consumed RAM went up from 90Mb to 110Mb and execution time from ~30ms to 200-300ms. To my surprise it didn't add DynamoDB into the map. No added value at all compared to the checkbox.
