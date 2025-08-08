$(document).ready(function (){
    $('#downloadBtn').click(function (){
        $('#loadingBlock').css('display', 'block');
        var type=$('#isFul').is(':checked')?1:0;
        let data=new FormData()
        data.append('file',$('#tariffFile').prop('files')[0]);
        data.append('tarifficatorType',type);
        $.ajax({
            url: '/Home/Download/',
            type: 'POST',
            cache: false,
            data: data,
            contentType: false,
            processData: false,
            success: function(data){
                $('#loadingBlock').css('display', 'none');
                if(!data.success){
                    alert(data.errors);
                }
            }
        });
    })
})

function switchTarifficatorType(){
    if($('#isFul').is(':checked')){
        $('#tarifficatorType').text('КТО');
    }else{
        $('#tarifficatorType').text('ФУЛ');
    }
}