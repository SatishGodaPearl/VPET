/*
-----------------------------------------------------------------------------
This source file is part of VPET - Virtual Production Editing Tool
http://vpet.research.animationsinstitut.de/
http://github.com/FilmakademieRnd/VPET

Copyright (c) 2018 Filmakademie Baden-Wuerttemberg, Animationsinstitut R&D Lab

This project has been initiated in the scope of the EU funded project 
Dreamspace under grant agreement no 610005 in the years 2014, 2015 and 2016.
http://dreamspaceproject.eu/
Post Dreamspace the project has been further developed on behalf of the 
research and development activities of Animationsinstitut.

This program is free software; you can redistribute it and/or modify it under
the terms of the MIT License as published by the Open Source Initiative.

This program is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the MIT License for more details.

You should have received a copy of the MIT License along with
this program; if not go to
https://opensource.org/licenses/MIT
-----------------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;

//!
//! This class rotates the light icon sprite to always face towards to camera and and scales them by the distance to the camera.
//! This class is beeing applied / should be attached on each light icon quad attached to any light source.
//!
namespace vpet
{
    public class LightIcon : MonoBehaviour
    {

        //!
        //! Scene object collider. Size is driven by this class.
        //!  
        BoxCollider targetCollider = null;
        public BoxCollider TargetCollider
        {
            set { targetCollider = value; }
        }


        //!
        //! Scene object scale. If game object is scaled, this value is used to keep icon size unique.
        //!
        Vector3 targetScale = Vector3.one;
        public Vector3 TargetScale
        {
            set
            {
                targetScale = value;
                targetScale = new Vector3(1f / targetScale.x, 1f / targetScale.y, 1f / targetScale.z);
            }
        }

        //!
        //! Use this for initialization
        //!
        void Start()
        {
            this.GetComponent<Renderer>().material.color = this.transform.parent.GetComponent<Light>().color;
        }

        //!
        //! Update is called once per frame
        //!
        void Update()
        {
            if (this.GetComponent<Renderer>())
            {
                Camera camera = Camera.main;
                Vector3 scale = targetScale * (Vector3.Distance(this.transform.position, camera.transform.position) / 30.0f) * (camera.fieldOfView / 30.0f);

                this.transform.rotation = camera.transform.rotation;
                this.transform.localScale = scale;

                // set the same scale to the light's collider
                if (targetCollider)
                {
                    targetCollider.size = scale;
                }
            }
        }
    }
}