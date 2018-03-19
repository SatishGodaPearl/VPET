﻿/*
-----------------------------------------------------------------------------
This source file is part of VPET - Virtual Production Editing Tool
http://vpet.research.animationsinstitut.de/
http://github.com/FilmakademieRnd/VPET

Copyright (c) 2018 Filmakademie Baden-Wuerttemberg, Institute of Animation

This project has been realized in the scope of the EU funded project Dreamspace
under grant agreement no 610005.
http://dreamspaceproject.eu/

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


#if USE_ARKIT
using System;
using System.Collections.Generic;
using vpet;

namespace UnityEngine.XR.iOS
{
	public class ARPlaneAlignment : MonoBehaviour
	{
		public Transform m_HitTransform;
		public float maxRayDistance = 300.0f;
		public LayerMask collisionLayer = 22;  //ARKitPlane layer
		private Vector3 m_initVector;
		private Quaternion m_initRotation = Quaternion.identity;

        bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
        {
            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
            if (hitResults.Count > 0) {
                foreach (var hitResult in hitResults) {
					m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform) * VPETSettings.Instance.trackingScale;
					if (m_initRotation == Quaternion.identity) {
						m_HitTransform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
						m_initRotation = m_HitTransform.rotation;
					}
					else
						m_HitTransform.rotation = m_initRotation;
                    //Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                    return true;
                }
            }
            return false;
        }
		
		// Update is called once per frame
		void Update () {
#if UNITY_EDITOR
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				
				//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
				//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
				if (Physics.Raycast (ray, out hit, maxRayDistance, collisionLayer)) {
					//and the rotation from the transform of the plane collider
					m_HitTransform.rotation = hit.transform.rotation;
				}
			}
#else
			if (Input.touchCount > 0 && m_HitTransform != null)
			{
				var touch = Input.GetTouch(0);
				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				if (Input.touchCount > 1) 
				{
					var touch1 = Input.GetTouch(1);
					Vector2 touchVec = (touch1.position - touch.position);
					Vector3 touchVec3 = new Vector3(touchVec.x, touchVec.y, 0.0f);
					if (touch1.phase == TouchPhase.Began) 
					{
						m_initRotation = m_HitTransform.rotation;
						Vector2 initVector2D = (touch1.position - touch.position);
						m_initVector = new Vector3(initVector2D.x, initVector2D.y, 0.0f);
					}
					else if (touch1.phase == TouchPhase.Moved)
					{
						float angle = Vector3.SignedAngle(touchVec3, m_initVector, new Vector3(0,0,1));
						m_HitTransform.rotation = m_initRotation * Quaternion.Euler (0, angle, 0);
					}
					else if (touch1.phase == TouchPhase.Ended)
					{
						m_initRotation = m_HitTransform.rotation;
					}
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
						ARPoint point = new ARPoint {
							x = screenPosition.x,
							y = screenPosition.y
						};
	                    // prioritize reults types
	                    ARHitTestResultType[] resultTypes = {
	                        //ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
	                        // if you want to use infinite planes use this:
	                        ARHitTestResultType.ARHitTestResultTypeExistingPlane,
	                        ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
	                        //ARHitTestResultType.ARHitTestResultTypeFeaturePoint
	                    }; 
						
	                    foreach (ARHitTestResultType resultType in resultTypes)
	                    {
	                        if (HitTestWithResultType (point, resultType))
	                        {
	                            return;
	                        }
	                    }
				}
			}
#endif

		}
	}
}
#endif