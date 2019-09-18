//function uploadFile() {
//    var input = document.getElementById("fileInput");
//    var thefile = input.files[0];
//    var formData = new FormData();
//    formData.append("file", thefile);
//    $.ajax({
//        url: 'https://localhost:44321/Book/upload',
//        type: 'POST',
//        data: formData,
//        processData: false,  // tell jQuery not to process the data
//        contentType: false,  // tell jQuery not to set contentType
//        success: function (result) {
//        },
//        error: function (jqXHR) {
//        },
//        complete: function (jqXHR, status) {
//        }
//    });
//}

//function postComment(bookId) {
//    const comment = {
//        Content: $("#postingComment").val(),
//        CFI: rendition.currentLocation().start.cfi;
//        BookId: bookId,
//    }

//    //$('#target').html('sending..');

//    $.ajax({
//        url: 'https://localhost:44321/Book/AddComment',
//        type: 'post',
//        data: JSON.stringify(comment),
//        dataType: 'json',
//        contentType: 'application/json',
//        success: function (data) {
//            //const commentBlock = $('div', { 'id': data.commentId })
//            //    .append($('p', { 'style': 'word-break:break-all'}).text(comment.Content)
//            //        .append($('button', { 'onclick': 'deleteComment(' + data.commentId + ')' }).text("delete")
//            //        ));

//            //const commentBlock = $('<div id="commentBlock10"></div >');
//            //commentBlock.appendto($('#comments'));

//            var commentBlock = $('<div class="commentBlock" id=commentBlock' + data.commentId + '></div >')
//                .append($("<p style='word-break:break-all'></p>").text(comment.Content))
//                .append($("<span style='float: left; left: 0px; font-size: x-small; color: gray'>"+ data.createTime +"</span>"))
//                .append($("<a href='#' class='btnDelete'>delete</a>")).on('click', function () {
//                    deleteComment(data.commentId);
//                });

//                //.append($("<p style='word-break:break-all'></p>").text(comment.Content))
//                //.append($("<p ></p>").text('3'))
//                //.append($("<p></p>").text('4'));
                
//            var tBody = $('#comments');
//            tBody.prepend(commentBlock);
//            //commentBlock.prependTo(tBody);
//            $("#postingComment").val("")
//        },
//        error: function (jqXHR) {
//        },
//        complete: function (jqXHR, status) {
//        }
//    });
//}
function deleteComment(commentId) {

    //$('#target').html('sending..');
    const comment = {
        CommentId: commentId,
    }
    $.ajax({
        url: 'https://localhost:44321/Book/DeleteComment',
        type: 'post',
        data: JSON.stringify(comment),
        contentType: 'application/json',
        success: function (data) {
            $('#commentBlock' + commentId).remove();
        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });
}

