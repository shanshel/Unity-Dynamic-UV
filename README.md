# Dynamic UV

Check this out on youtube!
[![Dynamic UV](https://img.youtube.com/vi/A8AX3aQDVqM/0.jpg)](https://www.youtube.com/embed/A8AX3aQDVqM)



# Table of Content

[Setup & How to use](#Setup-&-How-to-use)

[Where to find assets that support Dynamic UV ?](#Where-to-find-assets-that-support-Dynamic-UV)

[Make your own custom models work with Dynamic UV](#How-to-Make-your-own-custom-models-work-with-dynamic-UV)





# Setup & How to use

Dynamic UV should work just fine once you import it. 
\
 \
And here is how you can use it 



1. Click on any object that has Mesh Filter & Mesh Renderer in your scene  

![alt_text](https://shanshel.me/dynamicuv/readme_images/m1.jpg)

2. From the inspector Click on “Apply Shared Material” 

![alt_text](https://shanshel.me/dynamicuv/readme_images/m2.jpg)

3. New colors section will appear (The number of colors will depend on the model) 

![alt_text](https://shanshel.me/dynamicuv/readme_images/m3.jpg)

4. Change the colors to whatever you like!  


![alt_text](https://shanshel.me/dynamicuv/readme_images/m4.jpg)


# Where to find assets that support Dynamic UV

Currently, since the tool is new, and still in development

You can either 



* Use our assets packs: All of our assets packs in the asset store have Dynamic UV support.
* Easily adjust your custom model: you can adjust your own model to make it work with dynamic UV really easy. 



However, over time more developers and content creators will adopt this tool for their low-poly assets too.

So consider searching the store for the “Dynamic UV” tag to check if there are any awesome designers who have made use of our tool!


# How to Make your own custom models work with dynamic UV

You can easily make your models work with dynamic UV

And here is how:



1. Open your model in blender
2. Go to UV Editing Section 


![alt_text](https://shanshel.me/dynamicuv/readme_images/m5.jpg)

3. Now let’s say you want to implement **3 colors** in this model
4. We should select the vertices that represent “Color 1” 

![alt_text](https://shanshel.me/dynamicuv/readme_images/m6.jpg)


5. and then scale them down to 0, just like that  

![alt_text](https://shanshel.me/dynamicuv/readme_images/m7.jpg)
\
(Notice the little yellow dot, it’s after scaling down all the Vertices that should have the same color to 0)
6. Well, now do the same for other vertices too 

![alt_text](https://shanshel.me/dynamicuv/readme_images/m8.jpg)

As you can see now we have 3 little dots, that means we will have 3 different color options in Dynamic UV 

7. That’s it, save your model, import it, and it should work with Dynamic UV !
