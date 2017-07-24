 Add a 'Rigidbody2D' component to the spike ball.
 - Create a script **Spawner** under Assets/Code/Compenents/Life and paste the following:


Image sizes: 150, 300, 500, 700.  Prefer: 300 

'Script'

How:
<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>







TODO  - Why do some scripts have a check to disable and others do not. (add to first instance)


TODO paypal, patreon, and twitch links.

TODO
 - Image size is all over the place
 - say links are included when relevant er something.
 - Target character count on scripts is 65
 - Table of contents with anchors
 - Maybe copy paste for a fully expanded view all in one page (but note gifs make it not print friendly)
 - Lots of FAQs along the way, please consider these questions as you go, we try not to be redundant later on.
 - Stop saying cached for perf.
 - Post questions/comments under the youtube video (since git doesn't do discussions well). Maybe survey at end?
 - Brief bio / why i did this
 - Filenames in bold
 - Maybe always have a 'Why' or 'What' question after how.














Familiarize yourself with the Unity Editor a bit.  This [guide from Unity](https://docs.unity3d.com/Manual/LearningtheInterface.html) is a nice, quick overview.







More about special folders:

Everything under a folder named "Editor" is an editor script, including files in any subdirectories.  e.g. this script could be saved as Assets/Editor/AutoSave.cs or Assets/Code/Editor/Utils/AutoSave.cs  There are two special folder names that work anywhere in your folder hierarchy like this:
 - "[Editor](https://docs.unity3d.com/Manual/ExtendingTheEditor.html)": These scripts are only executed when in the Unity Editor (vs built into the game).
 - "[Resources](https://docs.unity3d.com/ScriptReference/Resources.html)": Assets bundled with the game, available for loading via code.  Note Unity [recommends using resources only for prototypes](https://unity3d.com/learn/tutorials/temas/best-practices/resources-folder) - the preferred solution is [AssetBundles](https://docs.unity3d.com/Manual/AssetBundlesIntro.html). 
 
The following are additional special folder names only considered when in the root Assets directory. e.g. Assets/Gizmos is a special directory but Assets/Code/Gizmos could hold anything.
 - "[Standard Assets](https://docs.unity3d.com/Manual/HOWTO-InstallStandardAssets.html)": These are optional assets and scripts available from Unity that you can add to your project anytime by right clicking in the Assets directory and selecting from 'Import Package'.
 - "[Editor Default Resources](https://docs.unity3d.com/ScriptReference/EditorGUIUtility.Load.html)": A resources directory only available to Editor scripts so you can have assets appear in the editor for debugging without increasing the game's built size.
 - "[Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html)": Editor-only visualizations to aid level design and debugging in the 'Scene' view.
 - "[Plugins](https://docs.unity3d.com/Manual/Plugins.html)": Dlls to include, typically from 3rd party libraries.
 - "[StreamingAssets](https://docs.unity3d.com/Manual/StreamingAssets.html)": Videos or other archives for your game to stream.

Be sure you do not use folders with these names anywhere in your project unless specifically for that Unity use case. e.g. Assets/Code/Editor/InGameMapEditor.cs may be intended to be part of the game but would be flagged as an Editor only script instead.