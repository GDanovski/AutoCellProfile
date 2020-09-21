dir = getDirectory("Choose a Directory ");
		setBatchMode(true);
       
       count = 0;
       countFiles(dir);
       n = 0;
       processFiles(dir);
       
       setBatchMode(false);
       print(count+" files processed");
       
function countFiles(dir) {
          list = getFileList(dir);
          for (i=0; i<list.length; i++) {
              if (endsWith(list[i], "/"))
                  countFiles(""+dir+list[i]);
              else
                  count++;
          }
      }
function processFiles(dir) {
          list = getFileList(dir);
          for (i=0; i<list.length; i++) {
              if (endsWith(list[i], "/"))
                  processFiles(""+dir+list[i]);
              else {
                 showProgress(n++, count);
                 path = dir+list[i];
                 processFile(path);
              }
          }
      }

function processFile(path) {
            if (endsWith(path, ".tif")) {
            	open(path);
            	var name = File.name;
				var newDir = File.directory;
            	var matrices =newDir +"\\Ready\\"+name + ".txt";
            	
            	if(!File.exists(newDir +"\\Ready")){
					File.makeDirectory(newDir +"\\Ready");
				}
				
				run("Gaussian Blur...", "sigma=2 stack");
				
				//save image
				name = replace(name, "_CompositeRegistred.tif", "");
				
				saveAs("Tiff", newDir +"\\Ready\\"+name+ "_blur.tif");
				print(newDir +"\\Ready\\"+name+ "_blur.tif" + " - processed!");
				close();
          }
      }