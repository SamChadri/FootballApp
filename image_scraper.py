import os
import csv
import requests
from bs4 import BeautifulSoup
import re 

# Target Roster URL (Example: Sidearm Sports Layout)
ROSTER_URL = "https://fightingillini.com/sports/football/roster"
HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
}

def scrape_roster(url):
    response = requests.get(url, headers=HEADERS)
    if response.status_code != 200:
        print(f"Failed to fetch page: Status {response.status_code}")
        return []

    soup = BeautifulSoup(response.content, "html.parser")
    players_data = []

    # Sidearm Sports usually wraps each player card in a 'sidearm-roster-player' class
    player_cards = soup.find_all(class_="s-person-card")
    
    if not player_cards:
        # Fallback to secondary Sidearm layout row structure
        player_cards = soup.find_all("tr", class_="sidearm-roster-player-row")
    counter = 0

    for card in player_cards:
        # 1. Extract Player Name
        name_tag = card.find(class_="s-person-details__personal") or card.find("a")
        name_heading = name_tag.find("h3")
        name = name_heading.text.strip() if name_heading else "Unknown"

        #Find details link
        details_tag = card.find("a")
        details_url = details_tag.get("href") or details_tag.get("src")

        if not details_url.startswith("http"):
            # Handle relative URLs by appending domain name
            base_domain = "/".join(url.split("/")[:3])
            details_url = base_domain + details_url

        #Navigate to details
        details_page = requests.get(details_url, headers=HEADERS)
        details_soup = BeautifulSoup(details_page.content, "html.parser")

        img_div = details_soup.find("div", class_="c-rosterbio__player__image")
        if not img_div:
            continue

        img_tag = img_div.find("img")
        if not img_tag:
            continue
            
        # Sidearm often uses lazy-loading attributes ('data-src' or 'src')
        img_url = img_tag.get("data-src") or img_tag.get("src")
        
        # Clean up thumbnail URLs to get full-resolution versions if possible
        if img_url:
            if not img_url.startswith("https"):
                # Handle relative URLs by appending domain name
                base_domain = "/".join(url.split("/")[:3])
                img_url = base_domain + img_url
            

        # 3. Extract Position/Jersey Number (Optional Metadata)
        jersey_tag = details_soup.find("div",class_="c-rosterbio__player__number")
        jersey = jersey_tag.text.strip() if jersey_tag else "N/A"

        players_data.append({
            "name": name,
            "jersey": jersey,
            "headshot_url": img_url
        })
        counter += 1
        print("Image URL: " + img_url)


        print("Progress: " + str(counter) + " out of " + str(len(player_cards)))

    return players_data

def rename_images(folder="headshots/2026"):
    if not os.path.exists(folder):
        os.makedirs(folder)
    
    files = os.listdir(folder)
    for file in files:

        filename = re.split(r'[_ ]+',file)
        print(filename)
        new_filename = ""
        for name in filename:
            new_filename += name + "_"
        
        new_filename = new_filename[:-2]


        os.rename(f"{folder}/{file}", f"{folder}/{new_filename}")

def download_images(players, folder="headshots"):
    if not os.path.exists(folder):
        os.makedirs(folder)
    counter = 0
    for player in players:
        url = player["headshot_url"]
        if not url or "placeholder" in url:
            continue
        
        # Clean name for a safe filename
        safe_name = player["name"].replace(" ", "_").replace("'", "").lower()
        safe_jersey = player["jersey"].replace(" ", "_").replace("'", "").lower()
        filename = f"{folder}/2026/{safe_jersey}_{safe_name}.webp"
        
        try:
            img_data = requests.get(url, headers=HEADERS).content
            with open(filename, "wb") as handler:
                handler.write(img_data)
            counter += 1
            print(f"Downloaded: {filename}")
            print("Progress: " + str(counter) + " out of " + str(len(players)))
        except Exception as e:
            print(f"Error downloading {player['name']}: {e}")

# --- Execution Flow ---
if __name__ == "__main__":
    print("Scraping roster metadata...")
    #roster_list = scrape_roster(ROSTER_URL)
    
    #if roster_list:
        #print(f"Found {len(roster_list)} players. Starting image download...")
        #download_images(roster_list)
    #else:
        #print("No players found. Check if the university uses a different web platform layout.")
    
    rename_images()
