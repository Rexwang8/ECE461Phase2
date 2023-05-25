# ECE461SoftwareEngineeringProject
ECE 461 - Purdue Package manager for Software Engineering class: Phase 2

Names: Rex Wang, Joseph Ma, Alan Zhang, Kevin Lin

wang5009 (at) purdue.edu

This is phase two of Purdue Package manager for Software Engineering class (our Phase 1 can be found here: https://github.com/Rexwang8/ECE461SoftwareEngineeringProject).

For phase 2, we will be passing our CLI to and inheriting from another team. We will be inheriting Team 28's CLI to build a frontend. Their original repository could be found here, https://github.com/ECE461Team/ECE461TeamRepo.

---

# Description

Our package manager works with NPM packages and exposes a RESTful api hosted on Google Cloud. We used a number of GC components like a SQL bigquery database, appengine, edgeless functions, cloud endpoints.

# Features

- Authentication and Authentication tokens
- Create packages with local files or with a url link to the repo
- Rate a package with static analyisis and 3rd party scoring
- Serve packages by endpoint
- Query packages by name, regex or id
- Version control to allow for multiple package versions under one project
- ADA complient frontend interface