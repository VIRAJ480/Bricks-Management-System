/**
 * ══════════════════════════════════════════════════════════════
 *  i18n.js  —  Language Translation Engine
 *  Supports: English (en) | Marathi (mr)
 *
 *  HOW IT WORKS:
 *  1. Every piece of text in the HTML has a  data-i18n="key"  attribute.
 *  2. This file holds a dictionary of all those keys in both languages.
 *  3. When the user picks a language from the dropdown, applyLanguage()
 *     loops through every element that has data-i18n and swaps its text.
 *  4. The chosen language is saved in localStorage so it persists when
 *     the user navigates to another page.
 * ══════════════════════════════════════════════════════════════
 */

// ── TRANSLATION DICTIONARY ──────────────────────────────────
// Add a new key here whenever you add a new data-i18n="key" in any view.
// Format:  "key": { en: "English text", mr: "मराठी मजकूर" }
const TRANSLATIONS = {

    // ── TOPBAR ──────────────────────────────────────────────
    "brand.name": { en: "Jyotirling", mr: "ज्योतिर्लिंग" },
    "brand.sub": { en: "Bricks Suppliers", mr: "वीट सप्लायर्स" },
    "topbar.activeLabour":  { en: "Active Labour",          mr: "सक्रिय कामगार" },
    "topbar.todayBricks":   { en: "Today's Bricks",         mr: "आजच्या विटा" },
    "topbar.pendingAmt":    { en: "Pending Amt",            mr: "बाकी रक्कम" },
    "topbar.logout":        { en: "↩ Logout",               mr: "↩ बाहेर पडा" },
    "lang.label":           { en: "🌐 Language",            mr: "🌐 भाषा" },

    // ── SIDEBAR GROUP LABELS ─────────────────────────────────
    "nav.overview":         { en: "Overview",               mr: "आढावा" },
    "nav.vendorMat":        { en: "Vendors & Materials",    mr: "विक्रेते आणि साहित्य" },
    "nav.labour":           { en: "Labour Management",      mr: "कामगार व्यवस्थापन" },
    "nav.production":       { en: "Production",             mr: "उत्पादन" },
    "nav.sales":            { en: "Sales & Payments",       mr: "विक्री आणि देयके" },
    "nav.reports":          { en: "Reports",                mr: "अहवाल" },
    "nav.system":           { en: "System",                 mr: "प्रणाली" },

    // ── SIDEBAR NAV ITEMS ────────────────────────────────────
    "nav.dashboard":        { en: "Dashboard",              mr: "डॅशबोर्ड" },
    "nav.vendorMaster":     { en: "Vendor Master",          mr: "विक्रेता मास्टर" },
    "nav.materialMaster":   { en: "Material Master",        mr: "साहित्य मास्टर" },
    "nav.materialPurchase": { en: "Material Purchase",      mr: "साहित्य खरेदी" },
    "nav.labourMaster":     { en: "Labour Master",          mr: "कामगार मास्टर" },
    "nav.labourAdvance":    { en: "Labour Advance",         mr: "कामगार आगाऊ" },
    "nav.workEntry":        { en: "Work Entry",             mr: "काम नोंद" },
    "nav.labourExpense":    { en: "Labour Expense",         mr: "कामगार खर्च" },
    "nav.advDeduction":     { en: "Advance Deduction",      mr: "आगाऊ कपात" },
    "nav.salaryPayment":    { en: "Salary Payment",         mr: "पगार देयक" },
    "nav.labourLedger":     { en: "Labour Ledger Report",   mr: "कामगार खातेवही अहवाल" },
    "nav.brickProduction":  { en: "Brick Production",       mr: "विटा उत्पादन" },
    "nav.customerMaster":   { en: "Customer Master",        mr: "ग्राहक मास्टर" },
    "nav.brickSales":       { en: "Brick Sales",            mr: "विटा विक्री" },
    "nav.customerPayment":  { en: "Customer Payment",       mr: "ग्राहक देयक" },
    "nav.materialReport":   { en: "Material Report",        mr: "साहित्य अहवाल" },
    "nav.labourReport":     { en: "Labour Report",          mr: "कामगार अहवाल" },
    "nav.productionReport": { en: "Production Report",      mr: "उत्पादन अहवाल" },
    "nav.salesReport":      { en: "Sales Report",           mr: "विक्री अहवाल" },
    "nav.pendingPayments":  { en: "Customer Pending Payments",       mr: "प्रलंबित देयके" },
    "nav.accountSettings":  { en: "Account Settings",       mr: "खाते सेटिंग्ज" },

    // ── DASHBOARD — WELCOME BANNER ───────────────────────────
    "dash.overview":        { en: "Here's your Brick Factory overview",  mr: "येथे आपल्या विटा कारखान्याचा आढावा आहे" },
    "dash.factoryActive":   { en: "Factory Active",         mr: "कारखाना सुरू" },

    // ── DASHBOARD — STAT CARDS ───────────────────────────────
    "dash.vendors":         { en: "Vendors",                mr: "विक्रेते" },
    "dash.manage":          { en: "Manage →",               mr: "व्यवस्थापित करा →" },
    "dash.materialTypes":   { en: "Material Types",         mr: "साहित्य प्रकार" },
    "dash.viewAll":         { en: "View All →",             mr: "सर्व पहा →" },
    "dash.activeLabour":    { en: "Active Labour",          mr: "सक्रिय कामगार" },
    "dash.bricksToday":     { en: "Bricks Today",           mr: "आजच्या विटा" },
    "dash.production":      { en: "Production →",           mr: "उत्पादन →" },
    "dash.customers":       { en: "Customers",              mr: "ग्राहक" },
    "dash.salesMonth":      { en: "Sales This Month",       mr: "या महिन्यातील विक्री" },
    "dash.viewSales":       { en: "View Sales →",           mr: "विक्री पहा →" },
    "dash.revenueMonth":    { en: "Revenue (This Month)",   mr: "महसूल (या महिन्यात)" },
    "dash.report":          { en: "Report →",               mr: "अहवाल →" },
    "dash.pendingPayments": { en: "Customer Pending Payments",       mr: "प्रलंबित देयके" },
    "dash.review":          { en: "Review →",               mr: "तपासा →" },
    "dash.LabourLedgerReport": { en: "Labour Ledger Report", mr: "कामगार खातेवही अहवाल" },
    "dash.AdvanceDeduction":   { en: "Advance Deduction",    mr: "आगाऊ कपात" },
    "dash.Breadbricks":        { en: "Bread Bricks",         mr: "ब्रेड विटा" },
    "dash.Solidbricks": { en: "Solid Bricks", mr: "ठोकळा विटा" },

    // ── DASHBOARD — RECENT BRICK SALES TABLE ─────────────────
    "dash.recentSales":     { en: "Recent Brick Sales",     mr: "अलीकडील विटा विक्री" },
    "dash.col.customer":    { en: "Customer",               mr: "ग्राहक" },
    "dash.col.bricksQty":   { en: "Bricks (Qty)",           mr: "विटा (संख्या)" },
    "dash.col.rate":        { en: "Rate",                   mr: "दर" },
    "dash.col.amount":      { en: "Amount",                 mr: "रक्कम" },
    "dash.col.status":      { en: "Status",                 mr: "स्थिती" },
    "dash.badge.paid":      { en: "Paid",                   mr: "भरले" },
    "dash.badge.partial":   { en: "Partial",                mr: "अंशतः" },
    "dash.badge.pending":   { en: "Pending",                mr: "प्रलंबित" },

    // ── DASHBOARD — RECENT MATERIAL PURCHASES TABLE ──────────
    "dash.recentPurchases": { en: "Recent Material Purchases", mr: "अलीकडील साहित्य खरेदी" },
    "dash.col.vendor":      { en: "Vendor",                 mr: "विक्रेता" },
    "dash.col.material":    { en: "Material",               mr: "साहित्य" },
    "dash.col.qty":         { en: "Qty",                    mr: "प्रमाण" },
    "dash.badge.due":       { en: "Due",                    mr: "देय" },

    // ── DASHBOARD — LABOUR OVERVIEW ──────────────────────────
    "dash.labourOverview":  { en: "Labour Overview",        mr: "कामगार आढावा" },
    "dash.badge.active":    { en: "Active",                 mr: "सक्रिय" },
    "dash.badge.onLeave":   { en: "On Leave",               mr: "रजेवर" },

    // ── DASHBOARD — ADVANCE SUMMARY ──────────────────────────
    "dash.advanceSummary":  { en: "Advance Summary",        mr: "आगाऊ सारांश" },
    "dash.totalAdvances":   { en: "Total Advances Given",   mr: "एकूण दिलेले आगाऊ" },
    "dash.thisMonth":       { en: "This month",             mr: "या महिन्यात" },
    "dash.deductionsDone":  { en: "Deductions Done",        mr: "कपात झाली" },
    "dash.recovered":       { en: "Recovered",              mr: "वसूल" },
    "dash.outstanding":     { en: "Outstanding Balance",    mr: "थकीत शिल्लक" },
    "dash.pendingRecovery": { en: "Pending recovery",       mr: "वसुली प्रलंबित" },

    // ── DASHBOARD — PRODUCTION TABLE ─────────────────────────
    "dash.productionWeek":  { en: "Production (This Week)", mr: "उत्पादन (या आठवड्यात)" },
    "dash.col.date":        { en: "Date",                   mr: "तारीख" },
    "dash.col.rawBricks":   { en: "Raw Bricks",             mr: "कच्च्या विटा" },
    "dash.col.fired":       { en: "Fired",                  mr: "भाजलेल्या" },
    "dash.col.rejected":    { en: "Rejected",               mr: "नाकारलेल्या" },
    "dash.today":           { en: "Today",                  mr: "आज" },
    "dash.yesterday":       { en: "Yesterday",              mr: "काल" },

    // ── DASHBOARD — MATERIAL STOCK ───────────────────────────
    "dash.materialStock":   { en: "Material Stock",         mr: "साहित्य साठा" },
    "dash.availableStock":  { en: "Available stock",        mr: "उपलब्ध साठा" },

    // ── DASHBOARD — PENDING CUSTOMER DUES ───────────────────
    "dash.pendingDues":     { en: "Pending Customer Dues",  mr: "प्रलंबित ग्राहक देणी" },
    "dash.days":            { en: "days",                   mr: "दिवस" },
    "dash.badge.unpaid":    { en: "Unpaid",                 mr: "न भरलेले" },

    // ── MATERIAL REPORT ──────────────────────────────────────
    "matrpt.title":         { en: "📦 MATERIAL PURCHASE REPORT", mr: "📦 साहित्य खरेदी अहवाल" },
    "matrpt.subtitle":      { en: "Filter by date range and generate material purchase summary", mr: "तारीख श्रेणीनुसार फिल्टर करा आणि साहित्य खरेदी सारांश तयार करा" },
    "matrpt.filterTitle":   { en: "REPORT FILTER",          mr: "अहवाल फिल्टर" },
    "matrpt.filterSub":     { en: "Select date range to generate report", mr: "अहवाल तयार करण्यासाठी तारीख श्रेणी निवडा" },
    "matrpt.fromDate":      { en: "From Date *",            mr: "पासून तारीख *" },
    "matrpt.toDate":        { en: "To Date *",              mr: "पर्यंत तारीख *" },
    "matrpt.generate":      { en: "🔍 Generate Report",     mr: "🔍 अहवाल तयार करा" },
    "matrpt.reset":         { en: "↺ Reset",                mr: "↺ रीसेट" },
    "matrpt.records":       { en: "Purchase Records",       mr: "खरेदी नोंदी" },
    "matrpt.selectRange":   { en: "Select a date range and click Generate", mr: "तारीख श्रेणी निवडा आणि तयार करा क्लिक करा" },
    "matrpt.print":         { en: "🖨️ Print",               mr: "🖨️ प्रिंट" },
    "matrpt.totalRecords":  { en: "Total Records",          mr: "एकूण नोंदी" },
    "matrpt.period":        { en: "Period",                 mr: "कालावधी" },
    "matrpt.grossTotal":    { en: "💰 Gross Total",         mr: "💰 एकूण बेरीज" },
    "matrpt.search":        { en: "Search by vendor, material, date…", mr: "विक्रेता, साहित्य, तारीखाने शोधा…" },
    "matrpt.col.no":        { en: "#",                      mr: "#" },
    "matrpt.col.date":      { en: "Purchase Date",          mr: "खरेदी तारीख" },
    "matrpt.col.vendor":    { en: "Vendor",                 mr: "विक्रेता" },
    "matrpt.col.material":  { en: "Material",               mr: "साहित्य" },
    "matrpt.col.qty":       { en: "Qty",                    mr: "प्रमाण" },
    "matrpt.col.rate":      { en: "Rate (₹)",               mr: "दर (₹)" },
    "matrpt.col.total":     { en: "Total Amount (₹)",       mr: "एकूण रक्कम (₹)" },
    "matrpt.grossLbl":      { en: "GROSS TOTAL",            mr: "एकूण बेरीज" },
    "matrpt.noSearch":      { en: "No records match your search.", mr: "आपल्या शोधाशी जुळणाऱ्या नोंदी नाहीत." },
    "matrpt.rows":          { en: "Rows:",                  mr: "ओळी:" },
    "matrpt.showing":       { en: "Showing",                mr: "दाखवत आहे" },
    "matrpt.of":            { en: "of",                     mr: "पैकी" },
    "matrpt.recordsLbl":    { en: "records",                mr: "नोंदी" },
    "matrpt.emptyDefault":  { en: "Select a date range above and click Generate Report to view purchase data.", mr: "वरील तारीख श्रेणी निवडा आणि खरेदी डेटा पाहण्यासाठी अहवाल तयार करा क्लिक करा." },
    "matrpt.emptyNoData":   { en: "No purchase records found for the selected date range.", mr: "निवडलेल्या तारीख श्रेणीसाठी कोणत्याही खरेदी नोंदी आढळल्या नाहीत." },

    // ── SALES REPORT ─────────────────────────────────────────
    "salesrpt.hint":        { en: "● Filter by date range to view sales revenue summary", mr: "● महसूल सारांश पाहण्यासाठी तारीख श्रेणीनुसार फिल्टर करा" },
    "salesrpt.filterTitle": { en: "REPORT FILTERS",         mr: "अहवाल फिल्टर" },
    "salesrpt.filterSub":   { en: "Select date range to view revenue summary", mr: "महसूल सारांश पाहण्यासाठी तारीख श्रेणी निवडा" },
    "salesrpt.fromDate":    { en: "From Date",              mr: "पासून तारीख" },
    "salesrpt.toDate":      { en: "To Date",                mr: "पर्यंत तारीख" },
    "salesrpt.generate":    { en: "🔍 Generate",            mr: "🔍 तयार करा" },
    "salesrpt.kpiCustomers":{ en: "Total Customers",        mr: "एकूण ग्राहक" },
    "salesrpt.kpiPeriod":   { en: "in selected period",     mr: "निवडलेल्या कालावधीत" },
    "salesrpt.kpiRevenue":  { en: "Total Revenue",          mr: "एकूण महसूल" },
    "salesrpt.kpiRevSub":   { en: "based on final amount",  mr: "अंतिम रकमेवर आधारित" },
    "salesrpt.summaryTitle":{ en: "REPORT SUMMARY",         mr: "अहवाल सारांश" },
    "salesrpt.summarySub":  { en: "Detailed breakdown of the generated report", mr: "तयार केलेल्या अहवालाचा तपशीलवार विभाजन" },
    "salesrpt.print":       { en: "🖨 Print Report",         mr: "🖨 अहवाल प्रिंट करा" },
    "salesrpt.fromDateLbl": { en: "From Date",              mr: "पासून तारीख" },
    "salesrpt.toDateLbl":   { en: "To Date",                mr: "पर्यंत तारीख" },
    "salesrpt.totalCust":   { en: "Total Customers",        mr: "एकूण ग्राहक" },
    "salesrpt.totalRev":    { en: "Total Revenue",          mr: "एकूण महसूल" },
    "salesrpt.generated":   { en: "Report Generated",       mr: "अहवाल तयार केला" },
    "salesrpt.empty":       { en: "Select a date range and click Generate to view the sales report.", mr: "विक्री अहवाल पाहण्यासाठी तारीख श्रेणी निवडा आणि तयार करा क्लिक करा." },

    // ── PENDING PAYMENT REPORT ───────────────────────────────
    "pendrpt.title":        { en: "PENDING PAYMENT REPORT", mr: "प्रलंबित देयक अहवाल" },
    "pendrpt.subtitle":     { en: "Track outstanding and partially paid customer balances", mr: "थकीत आणि अंशतः भरलेल्या ग्राहक शिल्लकांचा मागोवा घ्या" },
    "pendrpt.statCustDue":  { en: "Customers Due",          mr: "देय ग्राहक" },
    "pendrpt.statPending":  { en: "Fully Pending",          mr: "पूर्णपणे प्रलंबित" },
    "pendrpt.statPartial":  { en: "Partial",                mr: "अंशतः" },
    "pendrpt.statCollected":{ en: "Total Collected",        mr: "एकूण संकलित" },
    "pendrpt.statOutstanding":{ en: "Total Outstanding",    mr: "एकूण थकीत" },
    "pendrpt.tableTitle":   { en: "Outstanding Payments",   mr: "थकीत देयके" },
    "pendrpt.tableSub":     { en: "Customers with pending or partial balance", mr: "प्रलंबित किंवा अंशतः शिल्लक असलेले ग्राहक" },
    "pendrpt.search":       { en: "Search by customer name…", mr: "ग्राहकाच्या नावाने शोधा…" },
    "pendrpt.col.no":       { en: "#",                      mr: "#" },
    "pendrpt.col.customer": { en: "Customer",               mr: "ग्राहक" },
    "pendrpt.col.totalSales":{ en: "Total Sales",           mr: "एकूण विक्री" },
    "pendrpt.col.paid":     { en: "Paid",                   mr: "भरले" },
    "pendrpt.col.remaining":{ en: "Remaining",              mr: "उर्वरित" },
    "pendrpt.col.progress": { en: "Progress",               mr: "प्रगती" },
    "pendrpt.col.status":   { en: "Status",                 mr: "स्थिती" },
    "pendrpt.noSearch":     { en: "No records match your search.", mr: "आपल्या शोधाशी जुळणाऱ्या नोंदी नाहीत." },
    "pendrpt.empty":        { en: "No outstanding payments — all customers are fully paid!", mr: "कोणतेही थकीत देयके नाहीत — सर्व ग्राहकांनी पूर्ण भरणा केला आहे!" },
    "pendrpt.showing":      { en: "Showing",                mr: "दाखवत आहे" },
    "pendrpt.of":           { en: "of",                     mr: "पैकी" },
    "pendrpt.records":      { en: "records",                mr: "नोंदी" },

    // ── DASHBOARD — SALARY STATUS ────────────────────────────
    "dash.salaryStatus":    { en: "Salary Status",          mr: "पगार स्थिती" },
    "dash.salaryPaid":      { en: "Salary Paid",            mr: "पगार दिला" },
    "dash.workers32":       { en: "32 workers",             mr: "३२ कामगार" },
    "dash.badge.done":      { en: "Done",                   mr: "पूर्ण" },
    "dash.pendingSalary":   { en: "Pending Salary",         mr: "प्रलंबित पगार" },
    "dash.workers6":        { en: "6 workers",              mr: "६ कामगार" },
    "dash.totalPaidOut":    { en: "Total Paid Out",         mr: "एकूण दिलेला पगार" },

    // ── COMMON FORM / TABLE LABELS ───────────────────────────
    "common.active":        { en: "Active",                 mr: "सक्रिय" },
    "common.inactive":      { en: "Inactive",               mr: "निष्क्रिय" },
    "common.edit":          { en: "✏ Edit",                 mr: "✏ संपादित करा" },
    "common.delete":        { en: "🗑 Delete",              mr: "🗑 हटवा" },
    "common.cancel":        { en: "Cancel",                 mr: "रद्द करा" },
    "common.yesDelete":     { en: "Yes, Delete",            mr: "होय, हटवा" },
    "common.reset":         { en: "↺ Reset",                mr: "↺ रीसेट" },
    "common.cancelEdit":    { en: "↺ Cancel Edit",          mr: "↺ संपादन रद्द करा" },
    "common.showing":       { en: "Showing",                mr: "दाखवत आहे" },
    "common.of":            { en: "of",                     mr: "पैकी" },
    "common.records":       { en: "records",                mr: "नोंदी" },
    "common.actions":       { en: "Actions",                mr: "क्रिया" },
    "common.search":        { en: "Search",                 mr: "शोधा" },
    "common.total":         { en: "Total",                  mr: "एकूण" },
    "common.paid":          { en: "Paid",                   mr: "भरले" },
    "common.pending":       { en: "Pending",                mr: "प्रलंबित" },
    "common.details":       { en: "Details →",              mr: "तपशील →" },
    "common.all":           { en: "All →",                  mr: "सर्व →" },

    // ── SHARED FORM FIELD LABELS ─────────────────────────────
    "form.status":          { en: "Status",                 mr: "स्थिती" },
    "form.address":         { en: "Address",                mr: "पत्ता" },
    "form.mobileNumber":    { en: "Mobile Number",          mr: "मोबाइल नंबर" },
    "form.mobile":          { en: "Mobile",                 mr: "मोबाइल" },
    "form.date":            { en: "Date",                   mr: "तारीख" },
    "form.amount":          { en: "Amount (₹) *",           mr: "रक्कम (₹) *" },
    "form.reason":          { en: "Reason *",               mr: "कारण *" },
    "form.labour":          { en: "Labour *",               mr: "कामगार *" },
    "form.quantity":        { en: "Quantity *",             mr: "प्रमाण *" },
    "form.rate":            { en: "Rate (₹) *",             mr: "दर (₹) *" },
    "form.searchNameMobile":{ en: "Search by name or mobile…", mr: "नाव किंवा मोबाइलने शोधा…" },

    // ── VENDOR MASTER ────────────────────────────────────────
    "vendor.addVendor":     { en: "Add Vendor",             mr: "विक्रेता जोडा" },
    "vendor.editVendor":    { en: "Edit Vendor",            mr: "विक्रेता संपादित करा" },
    "vendor.updateVendor":  { en: "💾 Update Vendor",       mr: "💾 विक्रेता अपडेट करा" },
    "vendor.registerVendor":{ en: "＋ Register Vendor",     mr: "＋ विक्रेता नोंदवा" },
    "vendor.vendorName":    { en: "Vendor Name *",          mr: "विक्रेत्याचे नाव *" },
    "vendor.gstNumber":     { en: "GST Number",             mr: "जीएसटी क्रमांक" },
    "vendor.vendorList":    { en: "Vendor List",            mr: "विक्रेता यादी" },
    "vendor.vendorCol":     { en: "Vendor",                 mr: "विक्रेता" },
    "vendor.deleteTitle":   { en: "⚠️ Delete Vendor?",      mr: "⚠️ विक्रेता हटवायचा?" },

    // ── MATERIAL MASTER ──────────────────────────────────────
    "material.addMaterial":   { en: "Add Material",         mr: "साहित्य जोडा" },
    "material.editMaterial":  { en: "Edit Material",        mr: "साहित्य संपादित करा" },
    "material.updateMaterial":{ en: "💾 Update Material",   mr: "💾 साहित्य अपडेट करा" },
    "material.addBtn":        { en: "＋ Add Material",       mr: "＋ साहित्य जोडा" },
    "material.materialName":  { en: "Material Name *",      mr: "साहित्याचे नाव *" },
    "material.unit":          { en: "Unit *",               mr: "एकक *" },
    "material.materialList":  { en: "Material List",        mr: "साहित्य यादी" },
    "material.materialCol":   { en: "Material Name",        mr: "साहित्याचे नाव" },
    "material.unitCol":       { en: "Unit",                 mr: "एकक" },
    "material.deleteTitle":   { en: "⚠️ Delete Material?",  mr: "⚠️ साहित्य हटवायचे?" },

    // ── MATERIAL PURCHASE ────────────────────────────────────
    "purchase.newPurchase":   { en: "NEW PURCHASE",         mr: "नवीन खरेदी" },
    "purchase.editPurchase":  { en: "EDIT PURCHASE",        mr: "खरेदी संपादित करा" },
    "purchase.purchaseDate":  { en: "Purchase Date *",      mr: "खरेदी तारीख *" },
    "purchase.vendor":        { en: "Vendor *",             mr: "विक्रेता *" },
    "purchase.material":      { en: "Material *",           mr: "साहित्य *" },
    "purchase.pricing":       { en: "📋 Purchase Details",  mr: "📋 खरेदी तपशील" },
    "purchase.payment":       { en: "💳 Payment",           mr: "💳 देयक" },
    "purchase.paidAmount":    { en: "Paid Amount (₹)",      mr: "भरलेली रक्कम (₹)" },
    "purchase.pendingAmount": { en: "Pending Amount (Auto)",mr: "प्रलंबित रक्कम (स्वयं)" },
    "purchase.totalAmount":   { en: "Total Amount (Auto)",  mr: "एकूण रक्कम (स्वयं)" },
    "purchase.savePurchase":  { en: "＋ Save Purchase",      mr: "＋ खरेदी जतन करा" },
    "purchase.updatePurchase":{ en: "💾 Update Purchase",   mr: "💾 खरेदी अपडेट करा" },
    "purchase.purchaseList":  { en: "Purchase List",        mr: "खरेदी यादी" },
    "purchase.deleteTitle":   { en: "⚠️ Delete Purchase?",  mr: "⚠️ खरेदी हटवायची?" },

    // ── LABOUR MASTER ────────────────────────────────────────
    "labour.addLabour":       { en: "Add Labour",           mr: "कामगार जोडा" },
    "labour.editLabour":      { en: "Edit Labour",          mr: "कामगार संपादित करा" },
    "labour.addBtn":          { en: "＋ Add Labour",         mr: "＋ कामगार जोडा" },
    "labour.updateBtn":       { en: "💾 Update Labour",      mr: "💾 कामगार अपडेट करा" },
    "labour.labourName":      { en: "Labour Name *",        mr: "कामगाराचे नाव *" },
    "labour.dailyWage":       { en: "Daily Wage (₹)",       mr: "दैनिक मजुरी (₹)" },
    "labour.joiningDate":     { en: "Joining Date",         mr: "रुजू तारीख" },
    "labour.labourList":      { en: "Labour List",          mr: "कामगार यादी" },
    "labour.dailyWageCol":    { en: "Daily Wage",           mr: "दैनिक मजुरी" },
    "labour.joiningDateCol":  { en: "Joining Date",         mr: "रुजू तारीख" },
    "labour.deleteTitle":     { en: "⚠️ Delete Labour?",    mr: "⚠️ कामगार हटवायचा?" },

    // ── LABOUR ADVANCE ───────────────────────────────────────
    "advance.recordAdvance":  { en: "Record Advance",       mr: "आगाऊ नोंदवा" },
    "advance.editAdvance":    { en: "Edit Advance",         mr: "आगाऊ संपादित करा" },
    "advance.saveBtn":        { en: "＋ Record Advance",     mr: "＋ आगाऊ नोंदवा" },
    "advance.updateBtn":      { en: "💾 Update Advance",    mr: "💾 आगाऊ अपडेट करा" },
    "advance.advanceDate":    { en: "Advance Date *",       mr: "आगाऊ तारीख *" },
    "advance.advanceType":    { en: "Advance Type *",       mr: "आगाऊ प्रकार *" },
    "advance.advanceAmount":  { en: "Advance Amount (₹) *", mr: "आगाऊ रक्कम (₹) *" },
    "advance.advanceRecords": { en: "Advance Records",      mr: "आगाऊ नोंदी" },
    "advance.advanceType.col":{ en: "Advance Type",         mr: "आगाऊ प्रकार" },
    "advance.labourTotal":    { en: "Labour Total",         mr: "कामगार एकूण" },
    "advance.deleteTitle":    { en: "⚠️ Delete Advance?",   mr: "⚠️ आगाऊ हटवायचे?" },

    // ── LABOUR WORK ENTRY ────────────────────────────────────
    "work.workEntry":         { en: "WORK ENTRY",           mr: "काम नोंद" },
    "work.editEntry":         { en: "EDIT ENTRY",           mr: "नोंद संपादित करा" },
    "work.saveBtn":           { en: "＋ Save Entry",         mr: "＋ नोंद जतन करा" },
    "work.updateBtn":         { en: "💾 Update Entry",       mr: "💾 नोंद अपडेट करा" },
    "work.workDate":          { en: "Work Date *",          mr: "काम तारीख *" },
    "work.dailyWage":         { en: "Daily Wage (₹) *",     mr: "दैनिक मजुरी (₹) *" },
    "work.daysWorked":        { en: "Days Worked *",        mr: "काम केलेले दिवस *" },
    "work.totalSalary":       { en: "Total Salary (Auto)",  mr: "एकूण पगार (स्वयं)" },
    "work.workRecords":       { en: "WORK RECORDS",         mr: "काम नोंदी" },
    "work.daysCol":           { en: "Days",                 mr: "दिवस" },
    "work.totalSalaryCol":    { en: "Total Salary",         mr: "एकूण पगार" },
    "work.deleteTitle":       { en: "⚠️ Delete Entry?",     mr: "⚠️ नोंद हटवायची?" },

    // ── LABOUR EXPENSE ───────────────────────────────────────
    "expense.addExpense":     { en: "Add Expense",          mr: "खर्च जोडा" },
    "expense.editExpense":    { en: "Edit Expense",         mr: "खर्च संपादित करा" },
    "expense.saveBtn":        { en: "＋ Save Expense",       mr: "＋ खर्च जतन करा" },
    "expense.updateBtn":      { en: "💾 Update Expense",     mr: "💾 खर्च अपडेट करा" },
    "expense.expenseDate":    { en: "Expense Date *",       mr: "खर्च तारीख *" },
    "expense.expenseRecords": { en: "Expense Records",      mr: "खर्च नोंदी" },
    "expense.reasonCol":      { en: "Reason",               mr: "कारण" },
    "expense.deleteTitle":    { en: "⚠️ Delete Expense?",   mr: "⚠️ खर्च हटवायचा?" },

    // ── ADVANCE DEDUCTION ────────────────────────────────────
    "deduction.newDeduction": { en: "NEW DEDUCTION",        mr: "नवीन कपात" },
    "deduction.editDeduction":{ en: "EDIT DEDUCTION",       mr: "कपात संपादित करा" },
    "deduction.saveBtn":      { en: "＋ Save Deduction",     mr: "＋ कपात जतन करा" },
    "deduction.updateBtn":    { en: "💾 Update Deduction",   mr: "💾 कपात अपडेट करा" },
    "deduction.deductionDate":{ en: "Deduction Date *",     mr: "कपात तारीख *" },
    "deduction.deductionAmt": { en: "Deduction Amount (₹) *",mr: "कपात रक्कम (₹) *" },
    "deduction.deductionList":{ en: "Deduction List",       mr: "कपात यादी" },
    "deduction.deleteTitle":  { en: "⚠️ Delete Deduction?", mr: "⚠️ कपात हटवायची?" },

    // ── SALARY PAYMENT ───────────────────────────────────────
    "salary.newSalary":       { en: "NEW SALARY",           mr: "नवीन पगार" },
    "salary.editSalary":      { en: "EDIT SALARY",          mr: "पगार संपादित करा" },
    "salary.saveBtn":         { en: "＋ Save Salary",        mr: "＋ पगार जतन करा" },
    "salary.updateBtn":       { en: "💾 Update Salary",      mr: "💾 पगार अपडेट करा" },
    "salary.salaryDate":      { en: "Salary Date *",        mr: "पगार तारीख *" },
    "salary.totalSalary":     { en: "Total Salary (from Work Log)", mr: "एकूण पगार (कामाच्या नोंदीतून)" },
    "salary.totalExpense":    { en: "Total Expense (Deductions)", mr: "एकूण खर्च (कपात)" },
    "salary.finalSalary":     { en: "Final Salary (Auto)",  mr: "अंतिम पगार (स्वयं)" },
    "salary.paidAmount":      { en: "Paid Amount (₹) *",    mr: "भरलेली रक्कम (₹) *" },
    "salary.paymentStatus":   { en: "Payment Status *",     mr: "देयक स्थिती *" },
    "salary.salaryList":      { en: "Salary Payment List",  mr: "पगार देयक यादी" },
    "salary.expenseCol":      { en: "Expense",              mr: "खर्च" },
    "salary.finalCol":        { en: "Final Salary",         mr: "अंतिम पगार" },
    "salary.deleteTitle":     { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },

    // ── BRICK PRODUCTION ─────────────────────────────────────
    "prod.newEntry":          { en: "NEW ENTRY",            mr: "नवीन नोंद" },
    "prod.editEntry":         { en: "EDIT ENTRY",           mr: "नोंद संपादित करा" },
    "prod.saveBtn":           { en: "＋ Save Entry",         mr: "＋ नोंद जतन करा" },
    "prod.updateBtn":         { en: "💾 Update Entry",       mr: "💾 नोंद अपडेट करा" },
    "prod.productionDate":    { en: "Production Date *",    mr: "उत्पादन तारीख *" },
    "prod.brickType":         { en: "Brick Type *",         mr: "विटांचा प्रकार *" },
    "prod.productionRecords": { en: "Production Records",   mr: "उत्पादन नोंदी" },
    "prod.brickTypeCol":      { en: "Brick Type",           mr: "विटांचा प्रकार" },
    "prod.quantityCol":       { en: "Quantity",             mr: "प्रमाण" },
    "prod.deleteTitle":       { en: "⚠️ Delete Production Entry?", mr: "⚠️ उत्पादन नोंद हटवायची?" },

    // ── CUSTOMER MASTER ──────────────────────────────────────
    "customer.addCustomer":   { en: "ADD CUSTOMER",         mr: "ग्राहक जोडा" },
    "customer.editCustomer":  { en: "EDIT CUSTOMER",        mr: "ग्राहक संपादित करा" },
    "customer.saveBtn":       { en: "＋ Add Customer",       mr: "＋ ग्राहक जोडा" },
    "customer.updateBtn":     { en: "💾 Update Customer",    mr: "💾 ग्राहक अपडेट करा" },
    "customer.customerName":  { en: "Customer Name *",      mr: "ग्राहकाचे नाव *" },
    "customer.customerList":  { en: "Customer List",        mr: "ग्राहक यादी" },
    "customer.customerCol":   { en: "Customer",             mr: "ग्राहक" },
    "customer.deleteTitle":   { en: "⚠️ Delete Customer?",  mr: "⚠️ ग्राहक हटवायचा?" },

    // ── BRICK SALES ──────────────────────────────────────────
    "sales.newSale":          { en: "NEW SALE",             mr: "नवीन विक्री" },
    "sales.editSale":         { en: "EDIT SALE",            mr: "विक्री संपादित करा" },
    "sales.saveBtn":          { en: "＋ Save Sale",          mr: "＋ विक्री जतन करा" },
    "sales.updateBtn":        { en: "💾 Update Sale",        mr: "💾 विक्री अपडेट करा" },
    "sales.salesDate":        { en: "Sales Date *",         mr: "विक्री तारीख *" },
    "sales.customer":         { en: "Customer *",           mr: "ग्राहक *" },
    "sales.brickType":        { en: "Brick Type *",         mr: "विटांचा प्रकार *" },
    "sales.totalAmount":      { en: "Total Amount (Auto)",  mr: "एकूण रक्कम (स्वयं)" },
    "sales.paidAmount":       { en: "Paid Amount (₹)",      mr: "भरलेली रक्कम (₹)" },
    "sales.pendingAmount":    { en: "Pending Amount (Auto)",mr: "प्रलंबित रक्कम (स्वयं)" },
    "sales.salesList":        { en: "Sales List",           mr: "विक्री यादी" },
    "sales.deleteTitle":      { en: "⚠️ Delete Sale?",      mr: "⚠️ विक्री हटवायची?" },

    // ── CUSTOMER PAYMENT ─────────────────────────────────────
    "payment.newPayment":     { en: "NEW PAYMENT",          mr: "नवीन देयक" },
    "payment.editPayment":    { en: "EDIT PAYMENT",         mr: "देयक संपादित करा" },
    "payment.saveBtn":        { en: "+ Save Payment",       mr: "+ देयक जतन करा" },
    "payment.updateBtn":      { en: "💾 Update Payment",     mr: "💾 देयक अपडेट करा" },
    "payment.paymentDate":    { en: "Payment Date *",       mr: "देयक तारीख *" },
    "payment.customer":       { en: "Customer *",           mr: "ग्राहक *" },
    "payment.paymentMode":    { en: "Payment Mode *",       mr: "देयक पद्धत *" },
    "payment.paymentStatus":  { en: "Payment Status *",     mr: "देयक स्थिती *" },
    "payment.paymentList":    { en: "Payment List",         mr: "देयक यादी" },
    "payment.modeCol":        { en: "Mode",                 mr: "पद्धत" },
    "payment.deleteTitle":    { en: "⚠️ Delete Payment?",   mr: "⚠️ देयक हटवायचे?" },
};

// ── LANGUAGE ENGINE ─────────────────────────────────────────

/**
 * applyLanguage(lang)
 * Loops through every element with [data-i18n] and replaces
 * its textContent with the translation for the given language.
 * @param {string} lang  - "en" or "mr"
 */
function applyLanguage(lang) {
    // Find every element that has a data-i18n attribute
    document.querySelectorAll('[data-i18n]').forEach(el => {
        const key = el.getAttribute('data-i18n');

        // Look up the key in our dictionary
        if (TRANSLATIONS[key] && TRANSLATIONS[key][lang]) {
            el.textContent = TRANSLATIONS[key][lang];
        }
    });

    // Also update placeholder attributes (search boxes etc.)
    document.querySelectorAll('[data-i18n-placeholder]').forEach(el => {
        const key = el.getAttribute('data-i18n-placeholder');
        if (TRANSLATIONS[key] && TRANSLATIONS[key][lang]) {
            el.placeholder = TRANSLATIONS[key][lang];
        }
    });

    // Save the chosen language to localStorage so it persists across pages
    localStorage.setItem('brickErpLang', lang);

    // Update the dropdown to show the current selection
    const dropdown = document.getElementById('langDropdown');
    if (dropdown) dropdown.value = lang;
}

/**
 * changeLanguage(lang)
 * Called directly by the dropdown's onchange event.
 * @param {string} lang  - "en" or "mr"
 */
function changeLanguage(lang) {
    applyLanguage(lang);
}

/**
 * On every page load, read the saved language from localStorage
 * and apply it automatically — so the user's choice is remembered.
 */
document.addEventListener('DOMContentLoaded', function () {
    // Version bump — clears stale localStorage cache when translations change
    const I18N_VERSION = '3';
    if (localStorage.getItem('brickErpLangVer') !== I18N_VERSION) {
        localStorage.removeItem('brickErpLang');
        localStorage.setItem('brickErpLangVer', I18N_VERSION);
    }
    // Read saved language; default to English if nothing saved yet
    const savedLang = localStorage.getItem('brickErpLang') || 'en';
    applyLanguage(savedLang);
});
