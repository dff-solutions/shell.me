
shell.me
========

shell.me - your universal shell framework

What does shell.me try to solve?
================================

We often find us wrapping code in a minimalistic console app just to run it
with the OS task scheduler at a specific interval. That's a dump, repetitive and tiresome
task to do. It's getting even worse once you start to parse parameters again and again.

shell.me is our attempt to fix that.

What does shell.me offer?
=========================

- it provides a simple way to run any custom implemented command 
  (just create a dll project, implement a very simple interface and drop the dll
  into the shell.me directory)
  
- it does all the heavy lifting for you. No need to parse parameters yourself. Let's
  say your command foo needs an int argument (e.g. BatchSize). All you've got to do is
  to add an int property ```BatchSize``` to your command. Now you can use it like this:
  ```foo --batchsize=100```. And say you also want a boolean property ```Force```. Easy!

  You can either say: 
  
  ```foo --batchsize=100 --force``` //means force == true  
  ```foo --force=true``` //means force == true  
  ```foo --force=false``` // means force == false
  
- it is trivially extensible. For example: We parse a lot of common types for you.
  But if for instance, you want to use the ```Point``` type as a command argument, you
  need to give shell.me a hint. Just add a custom TypeProvider and add it to the
  collection of built in TypeProviders. That's the way to teach shell.me new tricks!
  After that you could use it like that for example: ```foo --Point={ X: 4, Y: 3}``` 
  
- it's layered into small chunks to provide rich flexibility
  
  ```ShellMe.CommandLine``` hosts the core functionality of shell.me
  That's a simple dll (not a console app!) that you actually can use to build a shell
  on top of it. For instance, you could build a html shell, a wpf shell - it's really
  up to you!
  
  ```ShellMe.Console``` a console app that uses ShellMe.CommandLine to provide a basic shell
  
  ```ShellMe.Testing``` provides useful helpers for testing.
  We care a lot about testing. That's why we also provide you with rich testing helpers.
  For instance, we have a TestConsole that you can use to write unit tests against custom commands.
  
- it provides a way to run commands in a non interactive way. Remember when we talked about
  using shell.me to run repetitive tasks with the OS task scheduler? You really don't wan't
  the shell.me process to stay open when using shell.me in such a scenario. 
  Just add ```--non-interactive``` as an argument to the command and shell.me will immediately
  shut down after the command has finished.
  
- it provides a rock solid way to prevent commands from running in parallel if you don't want that to happen!
  Let's say you schedule a command to run every 10 seconds. However, the time consumption of the command itself
  might be hard to predict and you don't want commands to overlap themselves. The OS task scheduler (at least
  on Windows) provides a way to avoid overlapping of tasks but it really only looks up for the process name not
  to clash. However, if you are using shell.me to schedule a bunch of commands, you don't want to use that option
  because you *do* want multiple instances of shell.me to overlap! You just don't want that to happen for specific
  commands! If you don't want that to happen just add ```--allow-parallel=false``` as an argument to the command.
  Shell.me automatically creates lock files in the shell.me directory that will prevent multiple instance of this
  command to run in parallel.
  
- It provides rich tracing features!
  We made things really easy for you. If you derive from BaseCommand you get all the rich tracing features
  from System.Diagnostic for free! In addition, you don't have to gamble with an app.config.xml file. If you like to log
  to the file system use ```--writeFile=foo.log``` and if you like to write to the system event log use ```--writeEventLog```.
  You can either leave the ```LogLevel``` untouched or set it globally (read: for File and EventLog) with
  e.g. ```--LogLevel=[Information, ActivityTracing]``` or use ```--FileLogLevel=[Error]``` and ```--EventLogLevel=[Information]```
  seperatly. Make sure you read up about all the different Levels: http://msdn.microsoft.com/de-de/library/system.diagnostics.sourcelevels.aspx
  
  - it's MIT licensed https://github.com/dff-solutions/shell.me/blob/master/LICENSE.md  
 