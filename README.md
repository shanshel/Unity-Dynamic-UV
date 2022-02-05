# Dynamic UV


# Table of Content


<table>
  <tr>
   <td><a href="#heading=h.2mcvygr6kyjq">Setup & How to use</a>
   </td>
  </tr>
  <tr>
   <td><a href="#heading=h.f9q27c8hv0fk">Where to find assets that support Dynamic UV ?</a>
   </td>
  </tr>
  <tr>
   <td><a href="#heading=h.djnotzu7hxdv">Make your own custom models work with Dynamic UV</a>
   </td>
  </tr>
</table>



# Setup & How to use

Dynamic UV should work just fine once you import it. \
 \
And here is how you can use it 



1. Click on any object that has Mesh Filter & Mesh Renderer in your scene  \


![alt_text](https://drive.google.com/uc?export=view&id=1NJL9LQh1uHirLaqtQ7lF9Vd8lspJnuEd)

2. From the inspector Click on “Apply Shared Material” \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \
 \

3. New colors section will appear (The number of colors will depend on the model) \

![alt_text](https://drive.google.com/uc?export=view&id=12DIe_qnviLumALGzDSs0M9bMotTWrE4j)

4. Change the colors to whatever you like!  \

![alt_text](https://drive.google.com/uc?export=view&id=1ijs5U1ZPtiU0bk-7b8ecESOzKn4HMBmm)


# Where to find assets that support Dynamic UV ?

Currently, since the tool is new, and still in development \
 \
You can either 



* Use our assets packs: All of our assets packs in the asset store have Dynamic UV support.
* Easily adjust your custom model: you can adjust your own model to make it work with dynamic UV really easy. \
[(Check the next section)](#heading=h.djnotzu7hxdv) \


However, over time more developers and content creators will adopt this tool for their low-poly assets too.

So consider searching the store for the “Dynamic UV” tag to check if there are any awesome designers who have made use of our tool!


# How to Make your own custom models work with dynamic UV 

You can easily make your models work with dynamic UV

And here is how:



1. Open your model in blender
2. Go to UV Editing Section \

![alt_text](https://drive.google.com/uc?export=view&id=1_0KiDnJMgTnefS6b_wMX-ldo9YHhMSq5)

3. Now let’s say you want to implement **3 colors** in this model
4. We should select the vertices that represent “Color 1” \

![alt_text](https://drive.google.com/uc?export=view&id=1MYZTDhzI6iCUCHMPLvH7DzLbDoKjYzOi)
\
 \
 \
 \
 \
 \
 \
 \
 \

5. and then scale them down to 0, just like that  \

![alt_text](https://drive.google.com/uc?export=view&id=1WkCLN0u20SpdOoZKp28wNuEpKopBZFfN)
\
(Notice the little yellow dot, it’s after scaling down all the Vertices that should have the same color to 0)
6. Well, now do the same for other vertices too \

![alt_text](https://drive.google.com/uc?export=view&id=1d-XEYBm20Gwg9aKIZyKyEFIrEIQJa5sx)
\
As you can see now we have 3 little dots, that means we will have 3 different color options in Dynamic UV \

7. That’s it, save your model, import it, and it should work with Dynamic UV !
