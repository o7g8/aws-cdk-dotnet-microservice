---
title: "Overview"
date: 2021-08-09T17:34:33+02:00
draft: false
weight: weight = 5
---

# Overview

You need to decide where your .NET development environment will be provisioned. You have following options:

- your local machine
- an EC2 VM
- a Workspaces machine

Please keep in mind, that a non-`metal` EC2 and Workspaces machines won't allow you to install Docker.

Therefore if you want to have Docker, you will need to install it on your local machine or on a `metal` EC2 (e.g. `m5zn.metal` or `z1d.metal`).

In case you go for an EC2, consider to use a spot instance with a permanent spot request to save costs. And use the latest `Microsoft Windows Server 2019 Base` AMI.

