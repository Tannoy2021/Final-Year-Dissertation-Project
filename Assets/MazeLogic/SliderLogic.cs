using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SliderLogic : MonoBehaviour
{

    public Slider gridSlider;
    //public Slider roomSizeSlider;
    public Slider algorithmSlider;
    public Slider roomMinSlider;
    public Slider roomMaxSlider;
    public Slider maximumRoomsSlider;
    public Slider generateRoomsSlider;
    public Slider randomWalkSlider;
    public MazeGenerator generator;


    private void Start()
    {
        if (generator != null)
        {
            if(gridSlider != null)
            {
                gridSlider.onValueChanged.AddListener(GridChange);
            }
            //if(roomSizeSlider != null)
            //{
            //    roomSizeSlider.onValueChanged.AddListener(RoomChange);
            //}
            if(algorithmSlider != null) 
            { 
                algorithmSlider.onValueChanged.AddListener(AlgorithmChange);
            }
            if(roomMinSlider != null) 
            {
                roomMinSlider.onValueChanged.AddListener(MinRoomChange);
            }
            if(roomMaxSlider != null) 
            {
                roomMaxSlider.onValueChanged.AddListener(MaxRoomChange);
            }
            if(maximumRoomsSlider != null)
            {
                maximumRoomsSlider.onValueChanged.AddListener(MaximumRoomChange);
            }
            if(generateRoomsSlider != null)
            {
                generateRoomsSlider.onValueChanged.AddListener(RoomGenerationChange);
            }
            if(randomWalkSlider != null)
            {
                randomWalkSlider.onValueChanged.AddListener(RandomWalkChange);
            }
        }
        else
        {
            Debug.LogError("Bad references");
        }
    }

    private void GridChange(float value)
    {
        generator.gridSize = (int)value;
    }
    //private void RoomChange(float value)
    //{
    //    generator.roomSize = (int)value;
    //}
    private void AlgorithmChange(float value)
    {
        generator.algorithmMethod= (int)value;
    }
    private void MinRoomChange(float value)
    {
        generator.roomMin = (int)value;
    }
    private void MaxRoomChange(float value)
    {
        generator.roomMax = (int)value;
    }
    private void MaximumRoomChange(float value)
    {
        generator.maximumRooms = (int)value;
    }
    private void RoomGenerationChange(float value)
    {
        generator.generateRooms = (int)value;
    }
    private void RandomWalkChange(float value)
    {
        generator.randomWalkSteps= (int)value;
    }

    //public void ChangeValues(float value)
    //{

    //    Debug.Log("Slider value changed: " + value);
    //    if (generator!= null) 
    //    {
    //        if (gameObject.name == "gridSize")
    //        {
    //            Debug.Log("Updating gridSize to: " + Mathf.RoundToInt(value));
    //            generator.gridSize = Mathf.RoundToInt(value);

    //        }
    //        if (gameObject.name == "roomSize")
    //        {
    //            Debug.Log("Updating roomSize to: " + Mathf.RoundToInt(value));
    //            generator.roomSize = Mathf.RoundToInt(value);
    //        }
    //        if (gameObject.name == "roomLimit")
    //        {
    //            Debug.Log("Updating roomLimit to: " + Mathf.RoundToInt(value));
    //            generator.maximumRooms = Mathf.RoundToInt(value);
    //        }
    //    }
    //}
}
