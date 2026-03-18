import i2v
from PIL import Image
import os
import sys
import json
#sys.argv[1]  r'D:\wallpaper\sese'
folder_path = sys.argv[1]
json_path = sys.argv[2]

BATCH_SIZE = 32
SCORE_THRESHOLD = 0.35


# ===== 加载模型 =====
illust2vec = i2v.make_i2v_with_chainer(
    "illust2vec_tag_ver200.caffemodel",
    "tag_list.json"
)
# ===== 扫描图片 =====
image_files = []

for root,dirs,files in os.walk(folder_path):
    for f in files:
        if f.lower().endswith((".jpg",".jpeg",".png",".webp")):
            image_files.append(os.path.join(root,f))
            
results_output = []

# ===== 分类映射 =====
categories = ["general", "character", "copyright", "rating"]

# ===== 分批处理 =====
for i in range(0, len(image_files), BATCH_SIZE):

    batch_files = image_files[i:i+BATCH_SIZE]

    pil_images = []
    valid_paths = []
    
    for path in batch_files:

        try:
            img = Image.open(path).convert("RGB")
            pil_images.append(img)
            valid_paths.append(path)
        except:            
            continue

    if not pil_images:
        continue
        
    results = illust2vec.estimate_plausible_tags(pil_images)
        
    for idx,tag_dict in enumerate(results):
            
        image_path = valid_paths[idx]
            
        image_result = {
            "file_path": image_path,
            "general": [],
            "character": [],
            "copyright": [],
            "rating": []
        }
    
        for category in categories:

            tag_list = list(tag_dict[category])

            for tag, score in tag_list:

                if score < SCORE_THRESHOLD:
                    continue

                image_result[category].append({
                    "tag": tag,
                    "score": float(score)
                })

        results_output.append(image_result)	
        
        
        
# ===== JSON文件 =====		
with open(json_path, "w", encoding="utf-8") as f:
    json.dump(results_output, f, ensure_ascii=False)

print("done")	