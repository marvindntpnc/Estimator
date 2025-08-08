$(document).ready(function (){
    $('#downloadBtn').click(function (){
        $('#loadingBlock').css('display', 'block');
        var type=$('#isFul').is(':checked')?1:0;
        let data=new FormData()
        data.append('file',$('#tariffFile').prop('files')[0]);
        data.append('tarifficatorType',type);
        $.ajax({
            url: '/Home/UploadTarifficator/',
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
    $('#CreateEstimate').click(function (){
        $(this).attr('disabled','disabled');
        if(estimateItems.length > 0){
            const data={
                title: $('#EstimateTitle').val(),
                number: $('#EstimateNumber').val(),
                currencyRate:$('#EstimateCurrencyRate').val(),
                customerName: $('#EstimateCustomerName').val(),
                isDiscounts:$('#EstimateIsDiscounts').is(':checked'),
                items:estimateItems
            }
            $.ajax({
                url: '/Home/CreateEstimate/',
                type: 'POST',
                cache: false,
                data: data,
                success: function(data){
                    if(data.success){
                        alert('Смета сформирована')
                        $('#CreateEstimate').removeAttr('disabled');
                    }else{
                        alert(data.errors);
                        $('#CreateEstimate').removeAttr('disabled');
                    }

                }
            });
        }else{
            alert('Добавьте позиции')
            $(this).removeAttr('disabled');
        }
    })
})

var estimateItems=[];

function switchTarifficatorType(){
    if($('#isFul').is(':checked')){
        $('#tarifficatorType').text('КТО');
    }else{
        $('#tarifficatorType').text('ФУЛ');
    }
}

function removeItemFromEstimate(element){
    estimateItems.splice(estimateItems.findIndex(obj => obj.itemId == element.val()), 1);
    element.parent().html(`<button class="btn btn-warning" type="button" value="${element.val()}" data-type="${element.attr('data-type')}" id="${element.attr('data-type')}-${element.val()}" onclick="addItemToEstimate($(this))">Добавить</button>`)
}

function addItemToEstimate(element){
    console.log($('#estimateItemQty-'+element.val()),$('#estimateItemQty-'+element.val()).val())
    const item={
        ItemId: element.val(),
        Qty:$('#estimateItemQty-'+element.val()).val(),
        TarifficatorType: element.attr('data-type'),
    }
    estimateItems.push(item);

    element.parent().html(`<button class="btn btn-danger" type="button" value="${item.itemId}" data-type="${item.tarifficatorType}" id="${item.tarifficatorType}-${item.id}" onclick="removeItemFromEstimate($(this))">Удалить</button>`)
}


function getTarrifficatorItems(itemName,pageIndex, pageSize){
    const data={
        ItemName:itemName,
        PageIndex: pageIndex,
        PageSize: pageSize
    }
    $.ajax({
        url: '/Home/EstimateForming/',
        type: 'POST',
        cache: false,
        data: data,
        success: function(data){
            for (let i=0; i<data.fulTarifficator.length; i++){
                const item=data.fulTarifficator[i];
                $('#ful-table-body').append(`<tr>
                                        <td>
                                                <div class="form-check">
                                                        <button class="btn btn-warning" type="button" value="${item.id}" data-type="ful" id="ful-${item.id}" onclick="addItemToEstimate($(this))">Добавить</button>
                                                </div>
                                        </td>
                                        <td>${item.itemCode}</td>
                                        <td>${item.categoryName}</td>
                                        <td>${item.subCategoryName}</td>
                                        <td>${item.name}</td>
                                        <td>${item.description}</td>
                                        <td>${item.measure}</td>
                                        <td>${item.currencyType}</td>
                                        <td>${item.price}</td>
                                        <td><div class="form-check">
                                                        <input class="form form-control" type="number" min="0" value="1" id="estimateItemQty-${item.id}"/>
                                                </div></td>
                                </tr>`)

            }
            //     $('#ful-tarifficator-block').append(`<nav aria-label="ful-navigation">
            //         <ul class="pagination">
            //                 <li class="page-item"><button class="page-link ful-nav-page disabled" type="button">Назад</button></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page active" type="button">1</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">2</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">3</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page disabled" type="button">...</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">Следующая</button></li>
            //         </ul>
            // </nav>`)

            for (let i=0; i<data.ktoTarifficator.length; i++){
                const item=data.ktoTarifficator[i];
                $('#kto-table-body').append(`<tr>
                                <td>
                                    <div class="form-check">
                                        <button class="btn btn-warning" type="button" value="${item.id}" data-type="kto" id="kto-${item.id}" onclick="addItemToEstimate($(this))">Добавить</button>
                                    </div>   
                                </td>
                                <td>${item.itemCode}</td>
                                <td>${item.categoryName}</td>
                                <td>${item.subCategoryName}</td>
                                <td>${item.name}</td>
                                <td>${item.description}</td>
                                <td>${item.discount}</td>
                                <td>${item.measure}</td>
                                <td>${item.currencyType}</td>
                                <td>${item.price}</td>
                                <td><div class="form-check">
                                                        <input class="form form-control" type="number" min="0" value="1" id="estimateItemQty-${item.id}"/>
                                                </div></td>
                        </tr>`)
            }
            //     $('#kto-tarifficator-block').append(`<nav aria-label="ful-navigation">
            //         <ul class="pagination">
            //                 <li class="page-item"><button class="page-link ful-nav-page disabled" data-direction="prev" type="button">Назад</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page active" data-direction="prev" type="button">1</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">2</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">3</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page disabled" type="button">...</a></li>
            //                 <li class="page-item"><button class="page-link ful-nav-page" type="button">Следующая</a></li>
            //         </ul>
            // </nav>`)
        }
    });
}