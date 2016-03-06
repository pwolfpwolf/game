using UnityEngine;
using System.Collections;

public class Prompter : MonoBehaviour
{
    private static ServerObject serverObject;
    private static Army army;

    //----------- Enter a castle ------------------------------//

    public static void enterFriendlyCastle (ServerObject serverObject_, Army army_)
    {
        army = army_;
        serverObject = serverObject_;
        PrompterWindowScript.Show ("Do you want to enter the castle?", 
            "No", clickEnterFriendlyCastleNo, "Yes", clickEnterFriendlyCastleYes);
    }

    public static void clickEnterFriendlyCastleYes() {

        print("Entering the castle");
        Requester.savePlot(army.Id, 
            MasterController.GetPendingPlot(), 
            serverObject.ObjectId);

        // clear the plot list
        RenderPath.instance.Clear();
        MasterController.guiMode = MasterController.GUIMode.NORMAL;
    }

    public static void clickEnterFriendlyCastleNo() {
        MasterController.undoLastPlot();
    }

    //----------- Merge Armies ------------------------------//

    public static void mergeArmies (Army army_)
    {
        army = army_;

        PrompterWindowScript.Show ("Do merge armies?", 
            "No", clickMergeArmiesNo, "Yes", ClickMergeArmiesYes);
    }

    public static void ClickMergeArmiesYes() {

        print("Prompter ClickMergeArmiesYes()"); 
        MasterController.instance.GiveMoveOrder(army.Id);
        print("Close window");
        PrompterWindowScript.instance.MakeInvisable();
        //MasterController.guiMode = MasterController.GUIMode.NORMAL;
    }

    public static void clickMergeArmiesNo() {
        MasterController.undoLastPlot();
    }

}

