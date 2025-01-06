Hi fellow developer! :) 

First of all, thanks a bunch for your purchase, I really appreciate it!
If you have any problems or suggestions, feel free to write me at contact@alive-studios.io

## More detailed documentation online ##
Please find a more detailed description and video tutorials here:
https://zealous-system-734.notion.site/AR-Magic-bar-1-0-documentation-d9c9be0653ce4b919a845e3ac59060ae?pvs=25

## Sample Scenes ##
This tool includes sample scenes that showcase the AR Magic Bar in realistic use-cases.
Please find those sample scenes under AR Magic Bar / Sample Scenes in the Project window.

## Legal notice: ##
This asset was created by Tobias Reidl from Alive Studios. 
You are not allowed to redistribute or share this asset or parts of it in any way. You are allowed to use this asset within any of your Unity projects 
(commercial or non commercial) for the intended purposes. 

How to use this asset: 

## The basic setup ##
Make sure that you have already setup your AR Scene (AR Foundation or Lightship needs to be installed) and  meshing or plane 
detection needs to be enabled.

Note: The setup in 2021 LTS is a bit different, please refer to the Unity AR Foundation documentation.

Important: When you want to use AR plane detection make sure to add a AR Raycast manager to your XR origin. When using meshing 
or Non-AR 3D, make sure that your AR mesh or 3D objects have a collider attached. You can select plane detection or meshing in 
the AR Magic Bar asset later on. 

Non AR / 3D Scenes are supported too, however only in combination with AR Foundation installed as the Magic Bar uses namespaces of AR Foundation. 
You could use the Magic Bar in a normal full 3D project or switch between AR and 3D scenes , however AR Foundation needs to be installed.

## Add the magic bar to your scene ##
To use the AR Magic Bar, drag the prefab "AR Magic Bar X.X" from the project window to your scene.

In the inspector you can adjust a few settings:
  
- Use meshing or plane detection?
- Select object once and place on each click or deselect the object on first place?
- Should there be a hide icon?
- Do you want to use the pagination or inventory option?
- Should there be a paginate / inventory icon?
- How many items should be shown on each page (for the bottom bar)?

## Organise your assets : PlaceableObjectDatabase ##
To organise our assets we want to create a new database in which we can store them. 
For that we can right-click on a folder in the project view and select Create → AR Magic Bar → 
PlaceableObjectDatabase. We can now select the Database, give it a name and put it into the folder we want. 
Later on all of the placeable objects from this database will be stored in the same folder. 
We can have multiple databases to organise our assets.

Next let’s drag our database(s) into the AR Magic Bar inspector window under the Placement Bar Logic script. 

## Prepare your assets ##
It is very important to prepare your assets. First and foremost make sure that all assets have some kind of collider attached. 
AR Magic Bar will automatically attach a collider to your object if there is none, however I highly recommend to add 
one by yourself to ensure the collider matches the shape and size of the actual object. 

Additional to the prepared prefab, you will need a UI Icon.  This should by in squared format e.g. 512 x 512 pixel, but 
any size is ok. You can e.g. simply create a screenshot of your object , drag it into unity and set it to a 2D Sprite (Single). 
Make sure if applicable (e.g. in Unity 6) to set the sprite mode to “Single”.


## Add your assets to the AR Magic bar ##

Open the Prefab and Image Editor at the very top under Window → AR Magic Bar → Prefab and Image Editor 

Drag and drop the database we created into the "PlaceableObjectDatabase" and hit refresh.

Click "Add Pair". Now you can drag in your prefab (left side) and Icon (right side).
Make sure each Pair has a unique prefab and Icon. 

Finally click on "Create Placeable Objects".

After creating your objects you could potentially drag in another database, hit refresh and add new objects.

## Manage and adjust objects after creation : PlaceableObjects ## 
After creating new placeable objects in a database you will find the created objects in the same folder as your database. 
"..._placeable", prefab and the corresponding scriptable object "...placeableObject".

Clicking on the prefab itself you can adjust the 3d object size or look. 
You can either scale  the “_Placeable”, object itself or make any kind of adjustment to the 
3d object (second child), just make sure that it has the “PlacementObjectVisual” script attached. 

On the parent object “Cylinder1_Placeable”, you can adjust some settings for the placement bar.

You can adjust which transform options are available, if the object is selectable and if it should get a coloured overlay
when being selected.When you object has a shader attached that might move it (e.g. grass that is moving in the wind via shader), the coloured overlay will not work. 
 Please disable it in this case, or write a custom script that will freeze this movement on selected.
 
 You can also add custom interactions. 
 
 Selecting the corresponding scriptable object you will have some more options. Here you can 
    
    - adjust the name of the object
    - change the UI sprite
    - hide the object in the beginning (if you want the player to pick it up e.g. )
    - grey it out (e.g. if it needs to be unlocked)
    - add a number to the object to work with limited inventory
    - disable spawning



DONE! From here you can use and test the magic bar. 



### Add custom buttons and functions : Custom Interactions ##
For Custom interactions , please visit the official documentation page:
https://zealous-system-734.notion.site/AR-Magic-bar-1-0-documentation-d9c9be0653ce4b919a845e3ac59060ae
