# STEM-Surge-7.0

### Simply put... A few intelligent servers manage a dynamic array of workers that can perform any task on demand in a massively multi-threaded environment.

### STEM Management created Surge in 2009 seeking to solve common issues encountered in datacenters managing big data flows. Surge is an extendable platform designed to grow; it is intended to be customized for each customer given their enterprise specific goals.

### An introduction video can be found here (https://youtu.be/rMsPSUjCcsE) 

### The website (www.stemmanagement.com) has installers and more information.

### The code released here is running in production environments; it can be used directly, and serve as a set of examples on how developers might create their own extensions to the platform.


#### The platform is comprised of 3 parts:

> The Deployment Managers: these are the brains, discovering and assigning work

> The Branch Managers: these are the dumb workers, they do exactly what the Deployment Managers dictate

> The Message Fabric: this is the means of Manager/Branch communication using a custom message format


#### There are 3 ways to extend the platform:

> Custom Deployment Controllers: Extend the Deployment Managers

> Custom Instructions: Extend the Branch Managers

> Custom Messages: Extend/Customize communications


#### Notes:

> When a class type is prefixed with an underscore, prefer the type without the underscore.

> Custom DeploymentControllers should ALWAYS derive from STEM.Surge.DeploymentController or STEM.Surge.FileDeploymentController if they're not deriving from another DeploymentController

> Custom Instructions should ALWAYS derive from STEM.Surge.Instruction if they're not deriving from another Instruction

> When Instruction._Run() returns false the configured FailureAction will be invoked

> When Instruction._Run() adds to the Exceptions collection an error file will be created

> ONLY when Instruction._Run() returns false (regardless of the Exceptions collection state) will the configured FailureAction be invoked
