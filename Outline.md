<style>
.fieldsetContainer {
    height: 0;
    overflow: hidden;
    transition: height 400ms linear;
}

#toggle {
    display: none;
}

#toggle:checked ~ .fieldsetContainer {
    height: 50px;
}

label .arrow-dn { display: inline-block; }
label .arrow-up { display: none; }

#toggle:checked ~ label .arrow-dn { display: none; }
#toggle:checked ~ label .arrow-up { display: inline-block; }
</style>

# Create a new 2D project

![alt text](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "Logo Title Text 1")
![New Project screen](https://i.imgur.com/T2iZrmK.png "New Project")

<details>
<summary>How</summary>
![alt text](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "Logo Title Text 1")
![New Project screen](https://i.imgur.com/T2iZrmK.png "New Project")
TODO
</details>
<details>
<summary>Why</summary>
TODO
</details>


<div class='showHide'>
    <input type="checkbox" id="toggle" />
    
    <label for="toggle">
        <span class='expand'>
            <span class="changeArrow arrow-up">↑</span>
            <span class="changeArrow arrow-dn">↓</span>
            Line expand and collapse
        </span>
    </label>
    
    <div class="fieldsetContainer">
        <fieldset id="fdstLorem">
            Lorem ipsum...
        </fieldset>
    </div>
</div>
