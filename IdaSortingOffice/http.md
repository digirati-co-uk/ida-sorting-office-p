
* User loads external manifest into Sorty
* Sorty knows what the URI of the container for this manifest is (in this case it's just based on the external manifest name)
* Sorty queries PRESLEY to get any already created manifests from this container:

GET /presley/ida/localhost-47724_roll_M-1304_01_
...
HTTP/1.1 404 Could not find requested container

OK, no container exists for this manifest (or whatever container strategy you are using)

* User selects a start and end canvas on the source manifest
* User clicks "Create manifest"
* Sorty constructs a new manifest in the client, **copying** the canvases selected to the new canvas's sequence[0]
* Sorty can get a label from the user for the manifest, or (currently) generate one
* Sorty constructs the @id of the manifest by appending a manifest name to the container. It's the PUT Url rather than this @id that determines where PRESLEY stores it and what URL it will be available on. Maybe it's also an error if the @id and the PUT URL disagree
* Sorty adds a special service block to the manifest to signal to PRESLEY that it wants PRESLEY to mint new canvas @ids (see payload next)
* Sorty PUTs:

PUT /presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6

{
   "@context":"http://iiif.io/api/presentation/2/context.json",
   "@id":"http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6",
   "@type":"sc:Manifest",
   "label":"M-1304_01_ canvases 4-6",
   "service":{
      "profile":"https://dlcs.info/profiles/mintrequest"
   },
   "sequences":[
      {
         "@id":"http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_sequence0",
         "@type":"sc:Sequence",
         "label":"Default sequence",
         "canvases":[
            {
               "@id":"http://localhost:47724/roll/M-1304/01/canvas/5",
               "@type":"sc:Canvas",
               "thumbnail":{
                  "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0005/full/1005,/0/default.jpg",
                  "@type":"dcTypes:Image",
                  "format":"image/jpeg",
                  "height":1300,
                  "width":1005,
                  "service":{
                     "@context":"http://iiif.io/api/image/2/context.json",
                     "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0005",
                     "profile":[
                        "http://iiif.io/api/image/2/level0.json",
                        {
                           "formats":[
                              "jpg"
                           ],
                           "qualities":[
                              "color"
                           ],
                           "supports":[
                              "sizeByWhListed"
                           ]
                        }
                     ],
                     "width":1005,
                     "height":1300,
                     "sizes":[
                        {
                           "width":116,
                           "height":150
                        },
                        {
                           "width":309,
                           "height":400
                        },
                        {
                           "width":1005,
                           "height":1300
                        }
                     ]
                  }
               },
               "height":3072,
               "width":2376,
               "images":[
                  {
                     "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005/anno",
                     "@type":"oa:Annotation",
                     "motivation":"sc:painting",
                     "resource":{
                        "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005/full/!2000,2000/0/default.jpg",
                        "@type":"dcTypes:Image",
                        "height":3072,
                        "width":2376,
                        "service":{
                           "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005",
                           "protocol":"http://iiif.io/api/image",
                           "height":3072,
                           "width":2376,
                           "profile":"http://iiif.io/api/image/2/level1.json"
                        }
                     },
                     "on":"http://localhost:47724/roll/M-1304/01/canvas/5"
                  }
               ],
               "service":{
                  "Average_confidence":87,
                  "Entity_stats":{
                     "DATE":"10",
                     "GPE":"20",
                     "NORP":"5",
                     "ORG":"15",
                     "PERSON":"1",
                     "TRIBE":"15"
                  },
                  "Full_text_length":2524,
                  "Spelling_accuracy":96,
                  "Total_entities_found":61,
                  "Typescript":true,
                  "id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005",
                  "@context":"https://dlcs-ida.org/ocr-info"
               }
            },
            {
               "@id":"http://localhost:47724/roll/M-1304/01/canvas/6",
               "@type":"sc:Canvas",
               "thumbnail":{
                  "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0006/full/1008,/0/default.jpg",
                  "@type":"dcTypes:Image",
                  "format":"image/jpeg",
                  "height":1300,
                  "width":1008,
                  "service":{
                     "@context":"http://iiif.io/api/image/2/context.json",
                     "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0006",
                     "profile":[
                        "http://iiif.io/api/image/2/level0.json",
                        {
                           "formats":[
                              "jpg"
                           ],
                           "qualities":[
                              "color"
                           ],
                           "supports":[
                              "sizeByWhListed"
                           ]
                        }
                     ],
                     "width":1008,
                     "height":1300,
                     "sizes":[
                        {
                           "width":116,
                           "height":150
                        },
                        {
                           "width":310,
                           "height":400
                        },
                        {
                           "width":1008,
                           "height":1300
                        }
                     ]
                  }
               },
               "height":3064,
               "width":2376,
               "images":[
                  {
                     "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006/anno",
                     "@type":"oa:Annotation",
                     "motivation":"sc:painting",
                     "resource":{
                        "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006/full/!2000,2000/0/default.jpg",
                        "@type":"dcTypes:Image",
                        "height":3064,
                        "width":2376,
                        "service":{
                           "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006",
                           "protocol":"http://iiif.io/api/image",
                           "height":3064,
                           "width":2376,
                           "profile":"http://iiif.io/api/image/2/level1.json"
                        }
                     },
                     "on":"http://localhost:47724/roll/M-1304/01/canvas/6"
                  }
               ],
               "service":{
                  "Average_confidence":86,
                  "Entity_stats":{
                     "DATE":"9",
                     "EVENT":"1",
                     "GPE":"13",
                     "LOC":"2",
                     "NORP":"5",
                     "ORG":"20",
                     "PERSON":"3",
                     "TRIBE":"28"
                  },
                  "Full_text_length":2951,
                  "Spelling_accuracy":91,
                  "Total_entities_found":60,
                  "Typescript":true,
                  "id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006",
                  "@context":"https://dlcs-ida.org/ocr-info"
               }
            },
            {
               "@id":"http://localhost:47724/roll/M-1304/01/canvas/7",
               "@type":"sc:Canvas",
               "thumbnail":{
                  "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0007/full/1027,/0/default.jpg",
                  "@type":"dcTypes:Image",
                  "format":"image/jpeg",
                  "height":1300,
                  "width":1027,
                  "service":{
                     "@context":"http://iiif.io/api/image/2/context.json",
                     "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0007",
                     "profile":[
                        "http://iiif.io/api/image/2/level0.json",
                        {
                           "formats":[
                              "jpg"
                           ],
                           "qualities":[
                              "color"
                           ],
                           "supports":[
                              "sizeByWhListed"
                           ]
                        }
                     ],
                     "width":1027,
                     "height":1300,
                     "sizes":[
                        {
                           "width":119,
                           "height":150
                        },
                        {
                           "width":316,
                           "height":400
                        },
                        {
                           "width":1027,
                           "height":1300
                        }
                     ]
                  }
               },
               "height":3048,
               "width":2408,
               "images":[
                  {
                     "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007/anno",
                     "@type":"oa:Annotation",
                     "motivation":"sc:painting",
                     "resource":{
                        "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007/full/!2000,2000/0/default.jpg",
                        "@type":"dcTypes:Image",
                        "height":3048,
                        "width":2408,
                        "service":{
                           "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007",
                           "protocol":"http://iiif.io/api/image",
                           "height":3048,
                           "width":2408,
                           "profile":"http://iiif.io/api/image/2/level1.json"
                        }
                     },
                     "on":"http://localhost:47724/roll/M-1304/01/canvas/7"
                  }
               ],
               "service":{
                  "Average_confidence":85,
                  "Entity_stats":{
                     "DATE":"3",
                     "GPE":"4",
                     "NORP":"6",
                     "ORG":"14",
                     "TRIBE":"5"
                  },
                  "Full_text_length":2627,
                  "Spelling_accuracy":94,
                  "Total_entities_found":38,
                  "Typescript":true,
                  "id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007",
                  "@context":"https://dlcs-ida.org/ocr-info"
               }
            }
         ]
      }
   ]
}

* PRESLEY processes the PUTted manifest. It sees the "mint new IDs" block.
* PRESLEY mints new @ids for the canvases. It creates a map of the new canvas @ids to the old ones.
* PRESLEY adds this service block to the manifest, and removes the mint request one. 
* PRESLEY keeps everything else about the submitted manifest, including any services the manifest had to start with
* PRESLEY returns the newly created manifest in the response (although Sorty ignores this)

* Sorty then redirects to the UV with ?manifest=(the new manifest @id)
* UV requests the manifest:

GET /presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6

* PRESLEY returns the manifest. Note the canvasmap service, which Sorty will use in a bit:

{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6",
  "@type": "sc:Manifest",
  "label": "M-1304_01_ canvases 4-6",
  "service": {
    "@id": "http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6/canvasmap",
    "@context": "https://dlcs.info/context/presley",
    "profile": "https://dlcs.info/profiles/canvasmap",
    "canvasmap": {
      "http://localhost:47724/canvases/3f9e1d3f95f94b08b293fb02a0a14e94": "http://localhost:47724/roll/M-1304/01/canvas/5",
      "http://localhost:47724/canvases/5cc8d52f949c45c2820162c0200fe02f": "http://localhost:47724/roll/M-1304/01/canvas/6",
      "http://localhost:47724/canvases/c20164e419f846cbabee25cda52be959": "http://localhost:47724/roll/M-1304/01/canvas/7"
    }
  },
  "sequences": [
    {
      "@id": "http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_sequence0",
      "@type": "sc:Sequence",
      "label": "Default sequence",
      "canvases": [
        {
          "@id": "http://localhost:47724/canvases/3f9e1d3f95f94b08b293fb02a0a14e94",
          "@type": "sc:Canvas",
          "thumbnail": {
            "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0005/full/1005,/0/default.jpg",
            "@type": "dcTypes:Image",
            "format": "image/jpeg",
            "height": 1300,
            "width": 1005,
            "service": {
              "@context": "http://iiif.io/api/image/2/context.json",
              "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0005",
              "profile": [
                "http://iiif.io/api/image/2/level0.json",
                {
                  "formats": [
                    "jpg"
                  ],
                  "qualities": [
                    "color"
                  ],
                  "supports": [
                    "sizeByWhListed"
                  ]
                }
              ],
              "width": 1005,
              "height": 1300,
              "sizes": [
                {
                  "width": 116,
                  "height": 150
                },
                {
                  "width": 309,
                  "height": 400
                },
                {
                  "width": 1005,
                  "height": 1300
                }
              ]
            }
          },
          "height": 3072,
          "width": 2376,
          "images": [
            {
              "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005/anno",
              "@type": "oa:Annotation",
              "motivation": "sc:painting",
              "resource": {
                "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005/full/!2000,2000/0/default.jpg",
                "@type": "dcTypes:Image",
                "height": 3072,
                "width": 2376,
                "service": {
                  "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005",
                  "protocol": "http://iiif.io/api/image",
                  "height": 3072,
                  "width": 2376,
                  "profile": "http://iiif.io/api/image/2/level1.json"
                }
              },
              "on": "http://localhost:47724/roll/M-1304/01/canvas/5"
            }
          ],
          "service": {
            "Average_confidence": 87,
            "Entity_stats": {
              "DATE": "10",
              "GPE": "20",
              "NORP": "5",
              "ORG": "15",
              "PERSON": "1",
              "TRIBE": "15"
            },
            "Full_text_length": 2524,
            "Spelling_accuracy": 96,
            "Total_entities_found": 61,
            "Typescript": true,
            "id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0005",
            "@context": "https://dlcs-ida.org/ocr-info"
          }
        },
        {
          "@id": "http://localhost:47724/canvases/5cc8d52f949c45c2820162c0200fe02f",
          "@type": "sc:Canvas",
          "thumbnail": {
            "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0006/full/1008,/0/default.jpg",
            "@type": "dcTypes:Image",
            "format": "image/jpeg",
            "height": 1300,
            "width": 1008,
            "service": {
              "@context": "http://iiif.io/api/image/2/context.json",
              "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0006",
              "profile": [
                "http://iiif.io/api/image/2/level0.json",
                {
                  "formats": [
                    "jpg"
                  ],
                  "qualities": [
                    "color"
                  ],
                  "supports": [
                    "sizeByWhListed"
                  ]
                }
              ],
              "width": 1008,
              "height": 1300,
              "sizes": [
                {
                  "width": 116,
                  "height": 150
                },
                {
                  "width": 310,
                  "height": 400
                },
                {
                  "width": 1008,
                  "height": 1300
                }
              ]
            }
          },
          "height": 3064,
          "width": 2376,
          "images": [
            {
              "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006/anno",
              "@type": "oa:Annotation",
              "motivation": "sc:painting",
              "resource": {
                "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006/full/!2000,2000/0/default.jpg",
                "@type": "dcTypes:Image",
                "height": 3064,
                "width": 2376,
                "service": {
                  "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006",
                  "protocol": "http://iiif.io/api/image",
                  "height": 3064,
                  "width": 2376,
                  "profile": "http://iiif.io/api/image/2/level1.json"
                }
              },
              "on": "http://localhost:47724/roll/M-1304/01/canvas/6"
            }
          ],
          "service": {
            "Average_confidence": 86,
            "Entity_stats": {
              "DATE": "9",
              "EVENT": "1",
              "GPE": "13",
              "LOC": "2",
              "NORP": "5",
              "ORG": "20",
              "PERSON": "3",
              "TRIBE": "28"
            },
            "Full_text_length": 2951,
            "Spelling_accuracy": 91,
            "Total_entities_found": 60,
            "Typescript": true,
            "id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0006",
            "@context": "https://dlcs-ida.org/ocr-info"
          }
        },
        {
          "@id": "http://localhost:47724/canvases/c20164e419f846cbabee25cda52be959",
          "@type": "sc:Canvas",
          "thumbnail": {
            "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0007/full/1027,/0/default.jpg",
            "@type": "dcTypes:Image",
            "format": "image/jpeg",
            "height": 1300,
            "width": 1027,
            "service": {
              "@context": "http://iiif.io/api/image/2/context.json",
              "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0007",
              "profile": [
                "http://iiif.io/api/image/2/level0.json",
                {
                  "formats": [
                    "jpg"
                  ],
                  "qualities": [
                    "color"
                  ],
                  "supports": [
                    "sizeByWhListed"
                  ]
                }
              ],
              "width": 1027,
              "height": 1300,
              "sizes": [
                {
                  "width": 119,
                  "height": 150
                },
                {
                  "width": 316,
                  "height": 400
                },
                {
                  "width": 1027,
                  "height": 1300
                }
              ]
            }
          },
          "height": 3048,
          "width": 2408,
          "images": [
            {
              "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007/anno",
              "@type": "oa:Annotation",
              "motivation": "sc:painting",
              "resource": {
                "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007/full/!2000,2000/0/default.jpg",
                "@type": "dcTypes:Image",
                "height": 3048,
                "width": 2408,
                "service": {
                  "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007",
                  "protocol": "http://iiif.io/api/image",
                  "height": 3048,
                  "width": 2408,
                  "profile": "http://iiif.io/api/image/2/level1.json"
                }
              },
              "on": "http://localhost:47724/roll/M-1304/01/canvas/7"
            }
          ],
          "service": {
            "Average_confidence": 85,
            "Entity_stats": {
              "DATE": "3",
              "GPE": "4",
              "NORP": "6",
              "ORG": "14",
              "TRIBE": "5"
            },
            "Full_text_length": 2627,
            "Spelling_accuracy": 94,
            "Total_entities_found": 38,
            "Typescript": true,
            "id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0007",
            "@context": "https://dlcs-ida.org/ocr-info"
          }
        }
      ]
    }
  ]
}

* User is happy.
* User goes back to Sorty, to look at the source manifest again
* Sorty again requests the container to see if there are any derived manifests. This time, it's not a 404 - the container is returned:

GET /presley/ida/localhost-47724_roll_M-1304_01_

* PRESLEY enumerates the manifests in the container and returns them as a collection:

{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "/presley/ida/localhost-47724_roll_M-1304_01_",
  "@type": "sc:Collection",
  "members": [
    {
      "@id": "http://localhost:47724/presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6",
      "@type": "sc:Manifest",
      "label": "M-1304_01_ canvases 4-6"
    }
  ]
}

* Sorty populates the drop down with this list
* User selects a manifest
* Sorty requests the manifest @id:
 
GET /presley/ida/localhost-47724_roll_M-1304_01_/manifest_4-6

(response body same as before)

* Sorty uses the canvas map to match the @ids in the returned manifest to the original ones
* It uses this info to highlight the canvases that were used to generate the chosen manifest.

## later

* You can load multiple manifests into Sorty
* It has a pluggable search component (put your widget that calls your search service in)
* You also plug in the strategy for naming containers, including letting the user name a container, browsing the existing containers, whatever
* You can label the manifest, but if you want to do more, switch to manifest editor for better control. You can add images, metadata, etc.
* Sorty stores some data of its own for info in an additional service block alongside the mintrequest. PRESLEY doesn't discard this one because it's some other service.


