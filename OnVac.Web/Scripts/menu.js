var menuShowed = true;

var mouseOver = false;
var mouseOut = false;

var pinLocked = true;

function Pin_Click(pin, pinBaseUrl, lockedImage, unlockedImage)
{
    pinLocked = !pinLocked;

    pin.src = pinBaseUrl + (pinLocked ? lockedImage : unlockedImage)
    pin.alt = (pinLocked ? "Sblocca" : "Blocca")
    
}

function HidMenu_MouseClick(menu, pin, hidMenu, type)
{
    setMenu(menu, pin, hidMenu, true, type, true)
}

function Menu_MouseOver(menu, pin, hidMenu, delay, type) 
{
    if (!pinLocked) 
    {
        mouseOver = true;
        mouseOut = false;

        setTimeout(function () { setMenu(menu, pin, hidMenu, true, type, false) }, delay)
    }

}

function Menu_MouseOut(menu, pin, hidMenu, delay, type) {

    if (!pinLocked) {

        mouseOut = true;
        mouseOver = false;

        setTimeout(function () { setMenu(menu, pin, hidMenu, false, type, false) }, delay)
    }
}


function setMenu(menu, pin, hidMenu, showMenu, type, force) 
{

    if (showMenu != menuShowed && (force || ((showMenu && !mouseOut) || (!showMenu && !mouseOver)))) 
    {
        menu.style.display = (showMenu ? '' : 'none');
        pin.style.display = (showMenu ? '' : 'none');
        hidMenu.style.display = (showMenu ? 'none' : '');

        var attributeValue;

        if (showMenu) 
        {
            attributeValue = (type == 0 ? 57 : 110);
        }
        else 
        {
            attributeValue = 12;
        }

        attributeValue = attributeValue + ',*';

        if (type == 0)
        {
            parent.document.getElementById('FsRow').rows = attributeValue
        }
        else
        {
            parent.document.getElementById('FsCol').cols = attributeValue
        }

        menuShowed = showMenu;

        mouseOut = false;
        mouseOver = false;
    }
}

function SuppressLeftFrame(enable) 
{
    if (enable)
        parent.document.getElementById('FsCol').cols = '0,*';
    else
        parent.document.getElementById('FsCol').cols = '110,*';
}


function listaBar_AfterItemSelected(listBar, itemSelected, evnt) 
{
    tryShowWaitScreen();
    //OnitLayoutSetTitle('', '');
	ClearTitle();
    riabilitaMenu(listBar, itemSelected, evnt);
}

function riabilitaMenu(listBar, itemSelected, evnt)
 {
    itemSelected.setSelected(false);
}

function LockUser() 
{
    OnitLayoutStatoMenu(true, null);
    window.parent.frames[2].location = './Login.aspx?LockedPageUrl=' + encodeURI(window.parent.frames[2].location);
}

function topMenu_ItemClick(url) 
{
	//OnitLayoutSetTitle('', '');
	ClearTitle();
	tryShowWaitScreen();
    top.frames[1].location = url;
}

function tryShowWaitScreen() 
{
    try 
    {
        top.frames[2].showWaitScreen(999999, true);
    }
    catch (err) 
    {
        // Non visualizza l'immagine di attesa e va avanti...
    }
}

function topMenu_LoadMain(url)
{
    //OnitLayoutSetTitle('', '');
	ClearTitle();
    tryShowWaitScreen();
    top.frames[2].location = url;
}
function ClearTitle()
{
	var Topf=getTopFrameMenu();
	if (Topf==null){return;}
	Topf.document.getElementById("lblNomeApp").innerHTML='';
	Topf.document.getElementById("lblNomeApp").className='';
}
function getTopFrameMenu() {
    var oParent, oFrame;
    var oLastParent = null;

    if (window == window.parent) return null;

    oParent = window.parent;

    oFrame = oParent.frames["TopFrame"];


    while (oParent != null && oFrame == null && oParent != oLastParent) {

        oLastParent = oParent;
        oParent = oParent.parent;
        if (oParent != null)
            oFrame = oParent.frames["TopFrame"];
    }

    return oFrame;
}