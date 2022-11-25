# Tekla Bridge Girder Plates

### Explanation of the Code

This is a code sample that:

  1. Builds contour plates, with

  2. Bolts. ...with the contour plates being tapered. This required a tiny bit of linear algebra.

All the inputs are read in from a CSV file.

I had to add a report which tells us if the plates are not planar. The bolts are all aligned to the plane of the plates. They need to be fairly accurate - 10 microns to be precise - according to the infinite wisdom of the Victorian Government engineer.

Hopefully the code is clear enough to give you something to start with.

### Demo:

[![Demo of Code via Video](https://user-images.githubusercontent.com/15097447/172506873-02b15b12-cbcb-4021-b97c-5a137cc7b5ac.png)](https://vimeo.com/446339309?embedded=true&source=video_title&owner=20292870)

### Warning: Gripe Alert

* We received an order to create bridge plate girders. 
* Victorian government engineer wanted these plates to a tolerance of 10 microns. Yes, you read that correctly.  But why this type of accuracy is necessary, or even possible, given we are building a bridge? 
* I must spent an additional 28 hours dealing with the back-and-forth rigmarole. Did not get paid. Anyways, it's water under the Westgate bridge (pun intended). 
* Here is the standard business model in the Victorian construction industry: (i) procure goods & services, and (ii) don't pay. 

* What could be more Victorian / Australian? Add in the cost of the imbeciles in our public service, blatant rorting + union headaches, and it's pretty easy to see why an infrastructure project should have a cost blow out of x5-10 its initial costing estimates making a $1 billion project into a $10 billion dollar project. 


Hopefully at least you gain something from the code. 






