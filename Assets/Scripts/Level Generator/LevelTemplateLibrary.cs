using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTemplateLibrary : MonoBehaviour {

    public static Vector2 RoomDimensions(LevelPiece._roomTypes type)
    {
        switch (type)
        {
            case LevelPiece._roomTypes.r5x4:
                return new Vector2(5, 4);
            case LevelPiece._roomTypes.r4x5:
                return new Vector2(4, 5);
            case LevelPiece._roomTypes.r7x3:
                return new Vector2(7, 3);
            case LevelPiece._roomTypes.r3x7:
                return new Vector2(3, 7);
            case LevelPiece._roomTypes.r10x10:
                return new Vector2(10, 10);
            case LevelPiece._roomTypes.r2x2:
            default:
                return new Vector2(2, 2);
        }
    }

    public static Vector2 RoomCenter(LevelPiece._roomTypes type)
    {
        switch (type)
        {
            case LevelPiece._roomTypes.r5x4:
                return new Vector2(2, 1.5f);
            case LevelPiece._roomTypes.r4x5:
                return new Vector2(1.5f, 2);
            case LevelPiece._roomTypes.r7x3:
                return new Vector2(3, 1);
            case LevelPiece._roomTypes.r3x7:
                return new Vector2(1, 3);
            case LevelPiece._roomTypes.r10x10:
                return new Vector2(4.5f, 4.5f);
            case LevelPiece._roomTypes.r2x2:
            default:
                return new Vector2(0.5f, 0.5f);
        }
    }

    public static int RoomMinExits(LevelPiece._roomTypes type)
    {
        switch (type)
        {
            case LevelPiece._roomTypes.r5x4:
                return 1;
            case LevelPiece._roomTypes.r4x5:
                return 1;
            case LevelPiece._roomTypes.r7x3:
                return 2;
            case LevelPiece._roomTypes.r3x7:
                return 2;
            case LevelPiece._roomTypes.r10x10:
                return 1;
            case LevelPiece._roomTypes.r2x2:
            default:
                return 1;
        }
    }

    public static int RoomMaxExits(LevelPiece._roomTypes type)
    {
        switch (type)
        {
            case LevelPiece._roomTypes.r5x4:
                return 5;
            case LevelPiece._roomTypes.r4x5:
                return 5;
            case LevelPiece._roomTypes.r7x3:
                return 4;
            case LevelPiece._roomTypes.r3x7:
                return 4;
            case LevelPiece._roomTypes.r10x10:
                return 4;
            case LevelPiece._roomTypes.r2x2:
            default:
                return 3;
        }
    }

    public static Vector2 HallDimensions(LevelPiece._hallTypes type)
    {
        switch(type)
        {
            case LevelPiece._hallTypes.h1x4:
                return new Vector2(1, 4);
            case LevelPiece._hallTypes.h4x1:
            default:
                return new Vector2(4, 1);
        }
    }

    public static Vector2 HallCenter(LevelPiece._hallTypes type)
    {
        switch (type)
        {
            case LevelPiece._hallTypes.h1x4:
                return new Vector2(0, 1.5f);
            case LevelPiece._hallTypes.h4x1:
            default:
                return new Vector2(1.5f, 0);
        }
    }

    public static int HallMinExits(LevelPiece._hallTypes type)
    {
        switch (type)
        {
            case LevelPiece._hallTypes.h1x4:
                return 2;
            case LevelPiece._hallTypes.h4x1:
            default:
                return 2;
        }
    }

    public static int HallMaxExits(LevelPiece._hallTypes type)
    {
        switch (type)
        {
            case LevelPiece._hallTypes.h1x4:
                return 2;
            case LevelPiece._hallTypes.h4x1:
            default:
                return 2;
        }
    }
}
