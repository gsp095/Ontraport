!function(){if(void 0===window.LP_TRACKING_LOADED){window.LP_TRACKING_LOADED=!0;for(var e=document.getElementsByTagName("meta"),t=0;t<e.length;t++){var n=e[t];switch(n.name){case"leadpages-meta-id":trackingId=n.content;break;case"leadpages-served-by":servedBy=n.content;break;case"leadpages-target-url":targetURL=n.content}}"undefined"==typeof servedBy&&(servedBy="leadpages");var a=(new Date).getTime(),l="xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,function(e){var t=(a+16*Math.random())%16|0;return("x"==e?t:7&t|8).toString(16)}),s=function(e,t){var n="//my.leadpages.net/analytics/pixel?",a=document.createElement("img");for(key in e)void 0!==e[key]&&(n+=encodeURIComponent(key)+"="+encodeURIComponent(e[key])+"&");a.src=n,t&&(a.onload=t)};if(s({id:trackingId,uuid:l,type:"view",served_by:servedBy}),"undefined"!=typeof targetURL){var i=document.getElementsByTagName("a");for(t=0;t<i.length;t++){var d=i[t];d.getAttribute("href")===targetURL&&(d.onclick=function(){return null!==d.getAttribute("clicked")||(d.setAttribute("clicked",!0),s({id:trackingId,uuid:l,type:"optin",served_by:servedBy},function(){window.location.href=targetURL})),!1})}}else{var r=document.getElementsByTagName("form");for(t=0;t<r.length;t++){var o=r[t];o.getAttribute("data-form-id")||(o.oldOnSubmit=o.onsubmit,o.onsubmit=function(){for(var e,t=this,n=document.getElementsByTagName("input"),a=null,i=null,d=0;d<n.length;d++){var r=n[d],o=r.getAttribute("name");o&&(-1!=o.toLowerCase().indexOf("email")&&(a=o,e=r.value),-1!=o.toLowerCase().indexOf("name")&&(i=o))}return a&&"undefined"!=typeof leadPagesIsValidEmail&&!leadPagesIsValidEmail(t,a)||i&&"undefined"!=typeof leadPagesIsValidName&&!leadPagesIsValidName(t,i)||i&&"undefined"!=typeof leadPagesIsValidFirstName&&!leadPagesIsValidFirstName(t,i)||s({id:trackingId,uuid:l,type:"optin",email:e,served_by:servedBy},function(){var e=!0;t.oldOnSubmit&&(e=t.oldOnSubmit.call(t)),"undefined"!=typeof HTMLElement&&e?HTMLFormElement.prototype.submit.call(t):e&&(t.onsubmit=function(){return!0},t.submit())}),!1})}}}}();