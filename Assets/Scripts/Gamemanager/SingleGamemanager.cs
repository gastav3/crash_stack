using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGamemanager : MonoBehaviour
{
        private static SingleGamemanager _instance;

        public static SingleGamemanager Instance { get { return _instance; } }

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }
    }