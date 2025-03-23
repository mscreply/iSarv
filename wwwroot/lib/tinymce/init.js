document.write(`
<script src="/lib/tinymce/tinymce.min.js"></script>
<script>
tinymce.init({
    mode : "exact",
    selector: "textarea.content-fa",
    plugins: 'print preview paste importcss searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount textpattern noneditable help charmap spellchecker visualblocks visualchars',
    menubar: false,
    toolbar:[ 'bold italic underline strikethrough | fontselect fontsizeselect styleselect | lineheight | alignleft aligncenter alignright alignjustify |  numlist bullist | pastetext', 'image media link anchor  | forecolor backcolor removeformat   | ltr rtl | hr pagebreak | charmap blockquote toc table| superscript  subscript | fullscreen  preview print | visualblocks visualchars code'],
    image_advtab: true ,
    branding: false,
    paste_data_images: false,
    automatic_uploads: false,
    document_base_url : "http://localhost:5001/",
    convert_urls: true,
    language : "fa",
    external_filemanager_path: "/shared/components/filemanager/",
    filemanager_title: "File Manager",
    external_plugins: {
        "responsivefilemanager": "filemanager/js/plugin_responsivefilemanager_plugin.js",
        "filemanager": "filemanager/js/plugin.js"
    },
    valid_children : "+body[style]",
    importcss_append: true,
    height: 300,
    image_caption: true,
    //font_formats: "",
    resize: true,
    toolbar_mode: 'sliding',
    contextmenu: "link table image",
    directionality: "rtl",
});
tinymce.init({
    mode : "exact",
    selector: "textarea.content-en",
    plugins: 'print preview paste importcss searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount textpattern noneditable help charmap spellchecker visualblocks visualchars',
    menubar: false,
    toolbar:[ 'bold italic underline strikethrough | fontselect fontsizeselect styleselect | lineheight | alignleft aligncenter alignright alignjustify |  numlist bullist | pastetext', 'image media link anchor  | forecolor backcolor removeformat   | ltr rtl | hr pagebreak | charmap blockquote toc table| superscript  subscript | fullscreen  preview print | visualblocks visualchars code'],
    image_advtab: true ,
    branding: false,
    paste_data_images: false,
    automatic_uploads: false,
    document_base_url : "http://localhost:5001/",
    convert_urls: true,
    language : "en",
    external_filemanager_path: "@Request.Scheme://@Request.Host/shared/components/filemanager/",
    filemanager_title: "File Manager",
    external_plugins: {
        "responsivefilemanager": "filemanager/js/plugin_responsivefilemanager_plugin.js",
        "filemanager": "filemanager/js/plugin.js"
    },
    valid_children : "+body[style]",
    importcss_append: true,
    height: 300,
    image_caption: true,
    //font_formats: "",
    resize: true,
    toolbar_mode: 'sliding',
    contextmenu: "link table image",
    directionality: "ltr",
});
</script>
`);