# Tech Details

> Latest Update: August 28, 2017

You are here, which shows you like building some software.
And here I'd like to introduce my technology stack and the reasons why I choose those to build this website.
If you have any suggestions, please submit an issue on my github repositories. Thank you for a lot.

## Preface

In the November and December of 2016, I learned some Python, HTML, JavaScript and jQuery and wrote the first version (maybe prototype?) of my blog [(Github Repository)](https://github.com/viccrubs/VicBlog-Lagecy) with Flask and jQuery and successfully deployed it on DigitalOcean. And then, using the some technologies, our team [Trap x00](https://github.com/trapx00) built [NJUAdmin](http://njuadmin.tk)(not working now) within a week. This stack is simple and easy to learn, but has some fatal disadvantages that forces me to rebuild it with better and modern technologies.

- Poor modularity
- Limited language features
- Difficulty in maintenance and further development
- Lack of modern work flow

Example is that when I decided to add the "check grades" function to NJUAdmin, I had to dive into the pure JavaScript scripts and add some duplicated functions.

This drove me to take a look at the stack I am using now.

## Stack Overview

### FrontEnd

[Github Repository](https://github.com/viccrubs/VicBlog-FrontEnd)

- [`React`](https://facebook.github.io/react/) 15.6.1
  - [`Redux`](http://redux.js.org/) for state management 3.7.2
    - [`Redux-Thunk`](https://github.com/gaearon/redux-thunk) for async actions. 2.2.0
  - [`React-Router`](https://github.com/ReactTraining/react-router) for route management 4.2.0
- [`TypeScript`](https://www.typescriptlang.org/) for better coding experiences 2.4.2
- [`Webpack`](https://webpack.js.org/) for bundling and useful dev utilities 3.5.5
- [`Ant design`](https://ant.design/) for UI components 2.12.8


### Backend

[Github Repository](https://github.com/viccrubs/VicBlog-Backend)

- [`Microsoft ASP.NET Core 2.0`](https://www.asp.net/core) running on [`.NET Core 2.0`](https://www.microsoft.com/net/core) 
  - [`Microsoft Entity Framework Core`](https://docs.microsoft.com/en-us/ef/core/) for ORM
- [`Microsoft SQL Server`](https://www.sqlite.org/) for database (used to be SQLite)
- [`Qiniu Cloud`](https://www.qiniu.com/) for storage

### Deployment and hosting

- [`Github Pages`](https://github.io) for frontend static webpage hosting
  - [`spa-github-pages`](https://github.com/rafrex/spa-github-pages) for a workaround to deploy SPA with `browserHistory`
- [`Microsoft Azure`](https://digitalocean.com) for backend deployment (used to be DigitalOcean)
- [`Cloudflare`](https://cloudflare.com) for DNS
- [`Dot TK`](https://dot.tk) for `.tk` free domain name

## Explanations

### Why Microsoft Azure?

1. I am a *Microsoft* fan. Microsoft technologies are my priorities.
2. Microsoft provides student with [`Microsoft Imagine`](https://imagine.microsoft.com/en-us/Catalog/Product/99) subscription which allows students to use some functions of Microsoft Azure, including Web Service and SQL Server for free. Despite the limitations, for example no SSL and limited storage, these are adequate for a simple personal blog.
3. DigitalOcean charges for at least 5 dollars per month, while Microsoft Azure are free.

### Why deploy frontend on `GitHub Page`?

`Github Page` has the following advantages:
1. Out-of-box use with zero configuration
2. Born with version control
3. High maintainability and reliability
4. Free of charge

What's more, Azure doesn't allow custom domain name for Student Subscription. So I had to deploy front end on another server.It's also a good chance to practice front end back end separation.

### Why TypeScript?

1. TypeScript is staticly, strongly typed which makes codes more safe. More errors can be found before and during compile, which is good for debugging.
2. TypeScript has lots of tools, like Visual Studio Code, and can flawlessly fit into my current workflow.
2. Compared with flow.js, TypeScript has much more complete [Type Definition Repository](https://github.com/DefinitelyTyped/DefinitelyType), which contains Type Definitions for lots of popular modules.
3. More frameworks and libraries are being built/rework to TypeScript.
3. Endorsed by Microsoft. Yeah don't forget I am a Microsoft fan.

Also, TypeScript, like an one-stop service, provides tools at once, which is not as flexive as the `Babel` things. To make the most of all these advantages, currently I am using `Babel` and `TypeScript` simultaneously.

### Why Qiniu?

1. Azure Storage are disabled for students. 
2. Qiniu is fast in mainland China.
3. Qiniu is free of charge as long as not exceeding the storage limitations.

### Why not apply server-side rendering?

Yes SSR is a pretty good solution for the cons of React SPA. But currently Ant Design seems to have problems on SSR. I [asked a question on StackOverflow](http://stackoverflow.com/questions/41911835/asp-net-core-spa-server-rendering-not-working-with-reactreduxant-designbabel) but earned `Tumbleweed` badge. which means "Asked a question with zero score, no answers, no comments, and low views for a week."

## To-dos and Plans

Nothing is perfect. There are still countless bugs and enhancements that can be done.

### Assets seperation and load on demand

All assets of frontend on my frontend are currently loaded once and for all, and all javascript are bundled into one single huge javascript. It wasn't supposed to be a problem until
1. you'd like to show some static but large contents which uses frontend items (icons, components etc.)
2. you'd like to load a javascript when it is demanded, instead of loading all assets when app is loading.
3. the bundle is too big
4. you'd like to use cdn

These are exactly the problems that I've encountered.

1. The about pages, including the one you are reading, are loaded and stored on the `server` as a `markdown` file so that I can update these without recompiling and republishing the frontend.
2. Storing on the server solves the first problem, but it compromises the abilities to use some awesome components and icons provided by `Ant Design`. I can only use pure markdown syntax and css to beautify the document.
3. The bundled js in my current version (2017-8-28) sizes at 5MB+, which is **insane**!! It significantly slows down the loading process and affects the experiences. 

There are several solutions on the Internet to achieve these goals, and I am reading them to solve these problems.

