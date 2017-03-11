# Tech Details

> Latest Update: Mar 9, 2017

You are here, which shows you like building some software, just like me.

## Preface

In the November and December of 2016, I learned some Python, HTML, JavaScript and jQuery and wrote the first version (maybe prototype?) of my blog [(Github Repository)](https://github.com/viccrubs/VicBlog-Lagecy) with Flask and jQuery and successfully deployed it on DigitalOcean. And then, using the some technologies, our team [Trap x00](https://github.com/trapx00) built [NJUAdmin](http://njuadmin.tk) within a week. This stack is simple and easy to learn, but has some fatal disadvantages that forces me to rebuild it with better and modern technologies.

- Poor modularity
- Limited language features
- Difficulty in maintenance and further development
- Lack of modern work flow

Example is that when I decided to add the "check grades" function to NJUAdmin, I had to dive into the pure JavaScript scripts and add some duplicated functions.

This drived me to take a look at the stack I am using now.

## Stack Overview

**FrontEnd**: [Github Repository](https://github.com/viccrubs/VicBlog-FrontEnd)

- [`React`](https://facebook.github.io/react/)
  - [`Redux`](http://redux.js.org/) for state management
    - [`Redux-Thunk`](https://github.com/gaearon/redux-thunk) for async actions. Have a plan for `redux-saga`
  - [`React-Router`](https://github.com/ReactTraining/react-router) for route management
- [`TypeScript`](https://www.typescriptlang.org/) for better coding experiences
- [`Webpack 2`](https://webpack.js.org/) for packaging and useful dev utilities
- [`Ant design`](https://ant.design/) for UI components


**BackEnd**: [Github Repository](https://github.com/viccrubs/VicBlog-Backend)
- [`Microsoft ASP.NET Core`](https://www.asp.net/core)
  - [`Microsoft Entity Framework Core`](https://docs.microsoft.com/en-us/ef/core/)
- [`SQLite`](https://www.sqlite.org/) for database 
- [`nginx`](https://nginx.org/en/) for reverse proxy


**Deployment and hosting**: 
- [`Github Pages`](https://github.io) for static webpage hosting
  - [`spa-github-pages`](https://github.com/rafrex/spa-github-pages) for a workaround to deploy SPA with `browserHistory`
- [`DigitalOcean`](https://digitalocean.com) for deployment
- [`Cloudflare`](https://cloudflare.com) for DNS with more utilities
- [`Dot TK`](https://dot.tk) for `.tk` free domain name

## FAQ

> Q: Why do you choose SQLite, instead of MySQL, MSSQL or others?

A: SQLite is easy and enough for hosting these small blog site.

> Q: Why deploy on `GitHub Page`, instead of your own server?

A: `Github Page` has the following advantages:
1. Out-of-box use with zero configuration
2. Born with version control
3. High maintainability and reliability

However, custom domain name doesn't support HTTPS enforcement, which is solved with [Cloudflare's flexive SSL](https://www.cloudflare.com/ssl/).

Compared with complicated nginx configuration, `Github Pages` provides a much more simple way for hosting my blog.

> Q: Why TypeScript?

A: EcmaScript 6 does great job on the evolution of JavaScript, but it's not enough. 
1. Coding on `Visual Studio Code` is so smoooooooooth and amazing with `InteliSense` !!
2. Compared with flow.js, TypeScript has much more complete [Type Definition Repository](https://github.com/DefinitelyTyped/DefinitelyType), which contains Type Definitions for lots of popular modules.
3. Endorsed by Microsoft and with an active community, TypeScript will not be abandoned.

Also, TypeScript, like an one-stop service, provides tools at once, which is not as flexive as the `Babel` things. To make the most of all these advantages, currently I am using `Babel` and `TypeScript` simultaneously.

> Q: Why not apply server-side rendering for better first-screen experience and SEO? Your API doesn't need to be exposed at all!

A: Yes SSR is a pretty good solution for the cons of React SPA. But currently Ant Design seems to have problems on SSR. I [asked a question on StackOverflow](http://stackoverflow.com/questions/41911835/asp-net-core-spa-server-rendering-not-working-with-reactreduxant-designbabel) but earned `Tumbleweed` badge. :(

Tumbleweed badge: Asked a question with zero score, no answers, no comments, and low views for a week.

