//TODO add up and down arrow key selection.
var itemPosition = 1;

var songInputId = "songToAdd";
var songListId = "songList";
var hiddenInputId = "hiddenInput";
var songListGroupId = "songListGroup";
var songSearchContainerId = "searchContainer"

var hiddenInput;
var songListGroup;
var songSearchContainer
var songListItems;
var songInput;

var selectedListItem;

$(document).ready(function () {
    hiddenInput = document.getElementById(hiddenInputId);
    songListGroup = document.getElementById(songListGroupId);
    songList = document.getElementById(songListId);
    songInput = document.getElementById(songInputId);
    songSearchContainer = document.getElementById(songSearchContainerId);
    var inputEventsCombined = function (e) {
        searchFunc();
        ShowSongList(songListGroup);
    }
    songSearchContainer.addEventListener('mouseleave', function (event) {
        songListGroup.setAttribute("data-toggle", "false");
       
    });

    $('#songToAdd').on('input', inputEventsCombined);
    songInput.addEventListener('keydown', function (e) { return KeyDownEventHandler(e) });
    songInput.addEventListener("keydown" , function (e) {

        if (e.keyCode == 13) {
            var firstSearchItem = songListGroup.getElementsByTagName('li');
            if (selectedListItem == null) {
                SetSelectedListItem(firstSearchItem[0])
            }
            else {
                postSongToAdd();
            }
           
        }
    })

    var inputClickEventFunctions = function () {
        if (songInput.value === "" || songInput.value === null) {
            return;
        }
        ShowSongList(songListGroup);
    }


    songInput.addEventListener('click', inputClickEventFunctions);
})



function InitializeListItems() {
    itemPosition = songListItems.length;
    Array.prototype.forEach.call(songListItems, s => {
        s.addEventListener('click', function () {
            SetSelectedListItem(s);
            postSongToAdd();
            
        });
        s.addEventListener("mouseenter", function (event) {
            SetSelectedListItem(s);
        });
    });
}

function InitializeItem(item) {
    item.dataset.display = 'true';
    item.dataset.highlight = 'false';
}

function HideSongListItem(ele) {
    if (ele == null) {
        return;
    }
    songListGroup.setAttribute("data-toggle", "false");
}

function ShowSongList(ele) {
    if (ele == null) {
        return;
    }
    songListGroup.setAttribute("data-toggle", "true");
}

function SetSelectedListItem(item) {
    if (selectedListItem == null) {
        selectedListItem = item;
        selectedListItem.dataset.highlight = 'true';
    }
    else {
        selectedListItem.dataset.highlight = 'false';
        selectedListItem = item;
        selectedListItem.dataset.highlight = 'true';
    }

}

function KeyDownEventHandler(e) {

    var items = songListGroup.getElementsByTagName("li");

    //backspace
    if (e.keyCode === 8) {
        if (songInput.value.length <= 1 || songInput.value === null) {
                HideSongListItem(songListGroup);
        }
    }

    //up
    if (e.keyCode === 38) {
        itemPosition--;

        if (itemPosition > items.length || itemPosition < 0) {
            itemPosition = 0;

        }

        SetSelectedListItem(items[itemPosition]);
    }
    //down
    else if (e.keyCode === 40) {
        itemPosition++;

        if (itemPosition > items.length || itemPosition < 0) {
            itemPosition = 0;
            
        }

        SetSelectedListItem(items[itemPosition]);
    }



    

}

var searchFunc = function () {
    var auth = $('#spotifyAuthCode').val();
    $.ajax({
        url: "/room/livesearch",
        type: "GET",
        async: true,
        data: {
            "auth": auth,
            "query": $('#songToAdd').val()
        },
        success: function (response) {

            if (response == null) {
                return;
            }

            songListGroup.innerHTML = response;
            songListItems = songListGroup.childNodes;
            InitializeListItems();
        },

        error: function (response) {
            console.log("Failed to get search results. " + response);
        }
    })
}

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(null, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(null, args);
    };
};

function postSongToAdd() {
    if (selectedListItem != null) {
        hiddenInput.value = selectedListItem.id;
        $.ajax({
            url: "/room/AddSongToRoom",
            type: "POST",
            data: {
                "CurrentRoom.Id": $("#roomId")[0].value,
                "CurrentRoom.SpotifyAuthCode": $("#spotifyAuthCode")[0].value,
                "SongToAdd": hiddenInput.value,
            },
            headers: {
                'Authorization': "Bearer " + $("#jwtToken")[0].value,
            },
            success: function (data) {
                $("#tableOfSongs").html(data);
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
}



