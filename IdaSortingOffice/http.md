
* User loads external _source_ manifest into Sorty (simple query string ?manifest=http://example.org/iiif/id1234567/manifest...)
  * TODO - pluggable source manifest selectors (search IIIF discovery server, search Wellcome, paste URL, drag drop)
  * TODO - Multiple manifests, rearrange
* Sorty decides on a collection name (a container in Presley) for the source manifest. Could be a hash of the name, in this case it's just based on the external manifest name.
  * TODO - pluggable strategy for determining collection name (including user input)
* The URI for the container in the examples below is http://localhost:47724/presley/ida/collection/_roll_T-58_1_
* Presley shouldn't care as long as the container name routes correctly; see http://iiif.io/api/presentation/2.1/#a-summary-of-recommended-uri-patterns 
* Sorty queries PRESLEY to get any already created manifests for the current source manifest:

`GET /presley/ida/collection/_roll_M-1304_01_`

`HTTP/1.1 404 Could not find requested container`

OK, no collection exists yet for this source manifest.

* User selects a start and end canvas on the source manifest
* User clicks "Create manifest"
* Sorty constructs a new manifest in the client, **copying** the canvases selected to the new canvas's sequence[0]
* Sorty can get a label from the user for the manifest, or (currently) generate one
* Sorty constructs the '@id' of the new manifest using IIIF naming conventions (knowing that Presley supports them).
* It's the PUT url rather than this @id that determines where PRESLEY stores it and what URL it will be available on. Maybe it's also an error if the @id and the PUT URL disagree
* Sorty adds a special service block to the manifest to signal to PRESLEY that it wants PRESLEY to mint new canvas @ids (see payload next)
  * `"service": { "profile":"https://dlcs.info/profiles/mintrequest" }`
* Sorty now needs to perform two separate operations. We won't worry about the transcational integrity of these for now.
  * PUT the new manifest to Presley, for storage
  * Tell Presley to add this manifest to the cource manifest collection. Sorty does this by POSTing a minimal JSON-LD chunk to the Presely Collection URL.
* First the Sorty PUT of the manifest:

`PUT /presley/ida/_roll_M-1304_01_cvs-19-20/manifest`

```json
{
   "@context":"http://iiif.io/api/presentation/2/context.json",
   "@id":"http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest",
   "@type":"sc:Manifest",
   "label":" roll M-1304 01 canvases 19-20",
   "service":{
      "profile":"https://dlcs.info/profiles/mintrequest"
   },
   "sequences":[
      {
         "@id":"http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/sequence/s0",
         "@type":"sc:Sequence",
         "label":"Default sequence",
         "canvases":[
            {
               "@id":"http://localhost:47724/roll/M-1304/01/canvas/20",
               "@type":"sc:Canvas",
               "thumbnail":{
                  "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0020/full/1033,/0/default.jpg",
                  "@type":"dcTypes:Image",
                  "format":"image/jpeg",
                  "height":1300,
                  "width":1033,
                  "service":{
                     "@context":"http://iiif.io/api/image/2/context.json",
                     "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0020",
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
                     "width":1033,
                     "height":1300,
                     "sizes":[
                        {
                           "width":119,
                           "height":150
                        },
                        {
                           "width":318,
                           "height":400
                        },
                        {
                           "width":1033,
                           "height":1300
                        }
                     ]
                  }
               },
               "height":3040,
               "width":2416,
               "images":[
                  {
                     "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020/anno",
                     "@type":"oa:Annotation",
                     "motivation":"sc:painting",
                     "resource":{
                        "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020/full/!2000,2000/0/default.jpg",
                        "@type":"dcTypes:Image",
                        "height":3040,
                        "width":2416,
                        "service":{
                           "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020",
                           "protocol":"http://iiif.io/api/image",
                           "height":3040,
                           "width":2416,
                           "profile":"http://iiif.io/api/image/2/level1.json"
                        }
                     },
                     "on":"http://localhost:47724/roll/M-1304/01/canvas/20"
                  }
               ],
               "service":{
                  "Average_confidence":67,
                  "Entity_stats":{

                  },
                  "Full_text_length":40,
                  "Spelling_accuracy":85,
                  "Total_entities_found":0,
                  "Typescript":true,
                  "id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020",
                  "@context":"https://dlcs-ida.org/ocr-info"
               }
            },
            {
               "@id":"http://localhost:47724/roll/M-1304/01/canvas/21",
               "@type":"sc:Canvas",
               "thumbnail":{
                  "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0021/full/1037,/0/default.jpg",
                  "@type":"dcTypes:Image",
                  "format":"image/jpeg",
                  "height":1300,
                  "width":1037,
                  "service":{
                     "@context":"http://iiif.io/api/image/2/context.json",
                     "@id":"https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0021",
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
                     "width":1037,
                     "height":1300,
                     "sizes":[
                        {
                           "width":120,
                           "height":150
                        },
                        {
                           "width":319,
                           "height":400
                        },
                        {
                           "width":1037,
                           "height":1300
                        }
                     ]
                  }
               },
               "height":3040,
               "width":2424,
               "images":[
                  {
                     "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021/anno",
                     "@type":"oa:Annotation",
                     "motivation":"sc:painting",
                     "resource":{
                        "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021/full/!2000,2000/0/default.jpg",
                        "@type":"dcTypes:Image",
                        "height":3040,
                        "width":2424,
                        "service":{
                           "@id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021",
                           "protocol":"http://iiif.io/api/image",
                           "height":3040,
                           "width":2424,
                           "profile":"http://iiif.io/api/image/2/level1.json"
                        }
                     },
                     "on":"http://localhost:47724/roll/M-1304/01/canvas/21"
                  }
               ],
               "service":{
                  "Average_confidence":57,
                  "Entity_stats":{

                  },
                  "Full_text_length":305,
                  "Total_entities_found":0,
                  "Typescript":false,
                  "id":"https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021",
                  "@context":"https://dlcs-ida.org/ocr-info"
               }
            }
         ]
      }
   ]
}
```


* PRESLEY processes the PUTted manifest. It sees the "mint new IDs" block.
* PRESLEY mints new @ids for the canvases. It creates a map of the new canvas @ids to the old ones.
* PRESLEY adds this service block to the manifest, and removes the mint request one. 
* PRESLEY keeps everything else about the submitted manifest, including any services the manifest had to start with
* PRESLEY returns the newly created manifest in the response (although Sorty ignores this)

Presley returns `HTTP/1.1 201 Created`

Now the second part of the "transaction". Sorty POSTs the new manifest to the collection for the current source manifest. As this is JSON-LD, it's just a graph, we only need to POST the '@id' and a '@type':

```
POST /presley/ida/collection/_roll_M-1304_01_

{
  "@id":"http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest",
  "@type":"sc:Manifest"
}
```

> TODO??? - if the POSTed manifest has fields like "label" or "description" that are different from the full manifest property values, what happens? You might want to have a different label for the manifest when you see the stub of it in a collection. Wellcome do this - it's "Volume 1" in the collection and "Medical Dictionary: volume 1" in the manifest. For now Presely just accepts @id and makes a row in a joining table somewhere. 

* Presley creates the collection at `/presley/ida/collection/_roll_M-1304_01_` if it doesn't exist
* Presley adds a reference to the manifest to it.

* Sorty then redirects to the UV with `?manifest=http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest` (the new manifest @id)
* UV requests the manifest:

`GET /presley/ida/_roll_M-1304_01_cvs-19-20/manifest`

* PRESLEY returns the manifest. 
* **Note the canvasmap service, which Sorty will use in a bit:**

```json
{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest",
  "@type": "sc:Manifest",
  "label": " roll M-1304 01 canvases 19-20",
  "service": {
    "@id": "http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest/canvasmap",
    "@context": "https://dlcs.info/context/presley",
    "profile": "https://dlcs.info/profiles/canvasmap",
    "canvasmap": {
      "http://localhost:47724/canvases/507448a7bb5a439997ba60003a5ba7cd": "http://localhost:47724/roll/M-1304/01/canvas/20",
      "http://localhost:47724/canvases/bf37ced0ae1249a78e5db88e1da28309": "http://localhost:47724/roll/M-1304/01/canvas/21"
    }
  },
  "sequences": [
    {
      "@id": "http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/sequence/s0",
      "@type": "sc:Sequence",
      "label": "Default sequence",
      "canvases": [
        {
          "@id": "http://localhost:47724/canvases/507448a7bb5a439997ba60003a5ba7cd",
          "@type": "sc:Canvas",
          "thumbnail": {
            "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0020/full/1033,/0/default.jpg",
            "@type": "dcTypes:Image",
            "format": "image/jpeg",
            "height": 1300,
            "width": 1033,
            "service": {
              "@context": "http://iiif.io/api/image/2/context.json",
              "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0020",
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
              "width": 1033,
              "height": 1300,
              "sizes": [
                {
                  "width": 119,
                  "height": 150
                },
                {
                  "width": 318,
                  "height": 400
                },
                {
                  "width": 1033,
                  "height": 1300
                }
              ]
            }
          },
          "height": 3040,
          "width": 2416,
          "images": [
            {
              "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020/anno",
              "@type": "oa:Annotation",
              "motivation": "sc:painting",
              "resource": {
                "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020/full/!2000,2000/0/default.jpg",
                "@type": "dcTypes:Image",
                "height": 3040,
                "width": 2416,
                "service": {
                  "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020",
                  "protocol": "http://iiif.io/api/image",
                  "height": 3040,
                  "width": 2416,
                  "profile": "http://iiif.io/api/image/2/level1.json"
                }
              },
              "on": "http://localhost:47724/roll/M-1304/01/canvas/20"
            }
          ],
          "service": {
            "Average_confidence": 67,
            "Entity_stats": {},
            "Full_text_length": 40,
            "Spelling_accuracy": 85,
            "Total_entities_found": 0,
            "Typescript": true,
            "id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0020",
            "@context": "https://dlcs-ida.org/ocr-info"
          }
        },
        {
          "@id": "http://localhost:47724/canvases/bf37ced0ae1249a78e5db88e1da28309",
          "@type": "sc:Canvas",
          "thumbnail": {
            "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0021/full/1037,/0/default.jpg",
            "@type": "dcTypes:Image",
            "format": "image/jpeg",
            "height": 1300,
            "width": 1037,
            "service": {
              "@context": "http://iiif.io/api/image/2/context.json",
              "@id": "https://dlcs-ida.org/thumbs/2/1/M-1304_R-01_0021",
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
              "width": 1037,
              "height": 1300,
              "sizes": [
                {
                  "width": 120,
                  "height": 150
                },
                {
                  "width": 319,
                  "height": 400
                },
                {
                  "width": 1037,
                  "height": 1300
                }
              ]
            }
          },
          "height": 3040,
          "width": 2424,
          "images": [
            {
              "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021/anno",
              "@type": "oa:Annotation",
              "motivation": "sc:painting",
              "resource": {
                "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021/full/!2000,2000/0/default.jpg",
                "@type": "dcTypes:Image",
                "height": 3040,
                "width": 2424,
                "service": {
                  "@id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021",
                  "protocol": "http://iiif.io/api/image",
                  "height": 3040,
                  "width": 2424,
                  "profile": "http://iiif.io/api/image/2/level1.json"
                }
              },
              "on": "http://localhost:47724/roll/M-1304/01/canvas/21"
            }
          ],
          "service": {
            "Average_confidence": 57,
            "Entity_stats": {},
            "Full_text_length": 305,
            "Total_entities_found": 0,
            "Typescript": false,
            "id": "https://dlcs-ida.org/iiif-img/2/1/M-1304_R-01_0021",
            "@context": "https://dlcs-ida.org/ocr-info"
          }
        }
      ]
    }
  ]
}
```

* User is happy.
* User goes back to Sorty, to look at the source manifest again
* Sorty again requests the collection to see if there are any derived manifests. This time, it's not a 404 - the collection is returned:

`GET /presley/ida/collection/_roll_M-1304_01_`

Presley returns:

```json
{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "http://localhost:47724/presley/ida/collection/_roll_M-1304_01_",
  "@type": "sc:Collection",
  "members": [
    {
      "@id": "http://localhost:47724/presley/ida/_roll_M-1304_01_cvs-19-20/manifest",
      "@type": "sc:Manifest",
      "label": " roll M-1304 01 canvases 19-20"
    }
  ]
}
```

(we didn't supply the `label` but it took it from the manifest stored earlier)

* Sorty populates the drop down with this list
* User selects a manifest
* Sorty requests the manifest @id:
 
`GET /presley/ida/_roll_M-1304_01_cvs-19-20/manifest`

(response body same as before)

* Sorty uses the canvas map to match the @ids in the returned manifest to the original ones
* It uses this info to highlight the canvases that were used to generate the chosen manifest.

## later

* You can load multiple manifests into Sorty
* It has a pluggable search component (put your widget that calls your search service in)
* You also plug in the strategy for naming containers, including letting the user name a container, browsing the existing containers, whatever
* You can label the manifest, but if you want to do more, switch to manifest editor for better control. You can add images, metadata, etc.
* Sorty stores some data of its own for info in an additional service block alongside the mintrequest. PRESLEY doesn't discard this one because it's some other service.


