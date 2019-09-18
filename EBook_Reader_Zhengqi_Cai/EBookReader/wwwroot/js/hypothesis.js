var fontSize = 1;
var coef = 1.25;
var label = document.getElementById("BookPath");
var bookUrl = label.textContent;
// Load the opf window.location.protocol +
var book = ePub(bookUrl, {
    canonical: function (path) {
        var url = window.location.href.replace(/loc=([^&]*)/, "loc=" + path);
        return url;
    }
});

var rendition = book.renderTo("viewer", {
    ignoreClass: "annotator-hl",
    width: "100%",
    height: "100%"
});


(function () {

    function postComment(bookId,cfi) {
        const comment = {
            Content: $("#postingComment").val(),
            CFI: cfi,
            BookId: bookId,
        }

        //$('#target').html('sending..');

        $.ajax({
            url: 'https://localhost:44321/Book/AddComment',
            type: 'post',
            data: JSON.stringify(comment),
            dataType: 'json',
            contentType: 'application/json',
            success: function (data) {
                //const commentBlock = $('div', { 'id': data.commentId })
                //    .append($('p', { 'style': 'word-break:break-all'}).text(comment.Content)
                //        .append($('button', { 'onclick': 'deleteComment(' + data.commentId + ')' }).text("delete")
                //        ));

                //const commentBlock = $('<div id="commentBlock10"></div >');
                //commentBlock.appendto($('#comments'));
                var deleteLink = $("<a href='#' class='btnDelete'>delete</a>").on('click', function () {
                    deleteComment(data.commentId);
                });
                var commentBlock = $('<div class="commentBlock" id=commentBlock' + data.commentId + '></div >')
                    .append($("<p style='word-break:break-all'></p>").text(comment.Content))
                    .append($("<span class='hidden'>"+ data.cfi+"</span>"))
                    .append($("<span style='float: left; left: 0px; font-size: x-small; color: gray'>" + data.createTime + "</span>"))
                    .append(deleteLink);
                //.append($("<p style='word-break:break-all'></p>").text(comment.Content))
                //.append($("<p ></p>").text('3'))
                //.append($("<p></p>").text('4'));

                var tBody = $('#comments');
                tBody.prepend(commentBlock);
                //($commentBlock).addEventListener('dblclick', function () {
                //    var cfi = $(this).closest('.hidden').textContent;
                //    rendition.display(cfi);
                //}, false);
                $(document).on('dblclick', '.commentBlock', function () {
                    var hidden = $(this).children('.hidden').first();
                    var cfi = hidden.text();
                    rendition.display(cfi);
                });
                //commentBlock.prependTo(tBody);
                $("#postingComment").val("")
            },
            error: function (jqXHR) {
            },
            complete: function (jqXHR, status) {
            }
        });
    }


  function start() {
    //  var label = document.getElementById("BookPath");
    //  var bookUrl = label.textContent;
    //// Load the opf window.location.protocol +
    //  var book = ePub(bookUrl, {
    //  canonical: function(path) {
    //    var url =  window.location.href.replace(/loc=([^&]*)/, "loc="+path);
    //    return url;
    //  }
    //  });

    //var rendition = book.renderTo("viewer", {
    //  ignoreClass: "annotator-hl",
    //  width: "100%",
    //  height: "100%"
    //});

    rendition.display(0);


    var next = document.getElementById("next");
    next.addEventListener("click", function(e){
      rendition.next();
      e.preventDefault();
    }, false);

    var prev = document.getElementById("prev");
    prev.addEventListener("click", function(e){
      rendition.prev();
      e.preventDefault();
    }, false);

    var nav = document.getElementById("navigation");
    var opener = document.getElementById("opener");
    opener.addEventListener("click", function(e){
      nav.classList.add("open");
    }, false);



    var closer = document.getElementById("closer");
    closer.addEventListener("click", function(e){
      nav.classList.remove("open");
    }, false);

    //// Hidden
    //var hiddenTitle = document.getElementById("hypothesisConfig");

    //rendition.on("rendered", function(section){
    //  var current = book.navigation && book.navigation.get(section.href);

    //  if (current) {
    //    document.title = current.label;
    //  }

    //  // TODO: this is needed to trigger the hypothesis client
    //  // to inject into the iframe
    //  requestAnimationFrame(function () {
    //    hiddenTitle.textContent = section.href;
    //  })

    //  var old = document.querySelectorAll('.active');
    //  Array.prototype.slice.call(old, 0).forEach(function (link) {
    //    link.classList.remove("active");
    //  })

    //  var active = document.querySelector('a[href="'+section.href+'"]');
    //  if (active) {
    //    active.classList.add("active");
    //  }

    //  let urlParam = params && params.get("url");
    //  let url = '';
    //  if (urlParam) {
    //    url = "url=" + urlParam + "&";
    //  }
    //  // Add CFI fragment to the history
    //  history.pushState({}, '', "?" + url + "loc=" + encodeURIComponent(section.href));
    //  // window.location.hash = "#/"+section.href
    //});

    var keyListener = function(e){

      // Left Key
      if ((e.keyCode || e.which) == 37) {
        rendition.prev();
      }

      // Right Key
      if ((e.keyCode || e.which) == 39) {
        rendition.next();
      }

    };

    rendition.on("keyup", keyListener);
    document.addEventListener("keyup", keyListener, false);

    book.loaded.navigation.then(function(toc){
      var $nav = document.getElementById("toc"),
          docfrag = document.createDocumentFragment();

      var processTocItem = function(chapter, parent) {
        var item = document.createElement("li");
        var link = document.createElement("a");
        link.id = "chap-" + chapter.id;
        link.textContent = chapter.label;
        link.href = chapter.href;
        item.appendChild(link);
        parent.appendChild(item);

        if (chapter.subitems.length) {
          var ul = document.createElement("ul");
          item.appendChild(ul);
          chapter.subitems.forEach(function(subchapter) {
            processTocItem(subchapter, ul);
          });
        }

        link.onclick = function(){
          var url = link.getAttribute("href");
          console.log(url)
          rendition.display(url);
          return false;
        };

      }

      toc.forEach(function(chapter) {
        processTocItem(chapter, docfrag);
      });

      $nav.appendChild(docfrag);


    });

    book.loaded.metadata.then(function(meta){
      var $title = document.getElementById("title");
      var $author = document.getElementById("author");
      var $nav = document.getElementById('navigation');

      $title.textContent = meta.title;
      $author.textContent = meta.creator;
      if ($nav.offsetHeight + 60 < window.innerHeight) {
        $nav.classList.add("fixed");
      }

    });

    //var tm;
    //function checkForAnnotator(cb, w) {
    // if (!w) {
    //   w = window;
    // }
    // tm = setTimeout(function () {
    //    if (w && w.annotator) {
    //      clearTimeout(tm);
    //      cb();
    //    } else {
    //      checkForAnnotator(cb, w);
    //    }
    //  }, 100);
    //}

    //book.rendition.hooks.content.register(function(contents, view) {
    //    checkForAnnotator(function () {

    //      var annotator = contents.window.annotator;

    //      contents.window.addEventListener('scrolltorange', function (e) {
    //        var range = e.detail;
    //        var cfi = new ePub.CFI(range, contents.cfiBase).toString();
    //        if (cfi) {
    //          book.rendition.display(cfi);
    //        }
    //        e.preventDefault();
    //      });


    //      annotator.on("highlightClick", function (annotation) {
    //        console.log(annotation);
    //        window.annotator.show();
    //      })

    //      annotator.on("beforeAnnotationCreated", function (annotation) {
    //        console.log(annotation);
    //        window.annotator.show();
    //      })

    //    }, contents.window);

    //});
      var mainBlock = document.getElementById("main");
      var commentFrame = document.getElementById("CommentsFrame");
      var commentOpener = document.getElementById("commentOpener");
      commentOpener.addEventListener("click", function (e) {
          if (commentFrame.style.display == "none") {
              commentFrame.style.setProperty("display", "flex");
              mainBlock.style.setProperty("width", "79vw");
          } else {
              commentFrame.style.setProperty("display", "none");
              mainBlock.style.setProperty("width", "100vw");

          }
      }, false);

      var btnLarge = document.getElementById("btnLarge");
      var btnSmall = document.getElementById("btnSmall");
      btnLarge.addEventListener("click", function (e) {
          fontSize *= coef;
          rendition.themes.fontSize(fontSize*100+'%');
      }, false);
      btnSmall.addEventListener("click", function (e) {
          fontSize /= coef;
          rendition.themes.fontSize(fontSize * 100 + '%');
      }, false);


      var cell = document.querySelectorAll('.commentBlock');
      for (var i = 0; i < cell.length; i++) {
          cell[i].addEventListener('dblclick', function () {
              var hidden = $(this).children('.hidden').first();
              var cfi = hidden.text();
              rendition.display(cfi);
          }, false);
      }

      var btnPost = document.getElementById("btnPost");
      var BookId = document.getElementById("BookId").textContent;
      btnPost.addEventListener("click", function (e) {
          var cfi = rendition.currentLocation().start.cfi;
          postComment(BookId,cfi);
      }, false);


    }

  document.addEventListener('DOMContentLoaded', start, false);
})();

