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
  to add an int property ```Batchsize``` to your command. Now you can use it like this:
  ```foo --batchsize=100```. And say you also want a boolean property ```Force```. Easy!
  You can either say: 
  ```foo --batchsize=100 --force``` //means force == true
  ```foo --force=true``` //means force == true
  ```foo --force=false``` // means force == false
 


