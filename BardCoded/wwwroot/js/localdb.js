class LocalDb {
    constructor(database, version) {
        this.database = database;
        this.version = version;
        this.request = null;
    }
    init = (upgrade) => {
        this.request = window.indexedDB.open("MyTestDatabase", 3);
        this.request.onerror = this.onerror;
        this.request.onsuccess = this.onsuccess;
        if (this.request.onupgradeneeded) {
            // This event is only implemented in recent browsers
            this.request.onupgradeneeded = upgrade;
        }
    }

    onerror = (event) => {
        console.error(`Database error: ${event.target.errorCode}`);
    }
    onsuccess = (event) => {
        console.error(`Database success: ${event}`);
    }
}

class BarcodeLocalDb extends LocalDb {
    constructor() {
        LocalDb.apply(this, "Bards", 1);
        this.init(upgradeIt);
    }
    upgradeIt = (event) => {
        const db = event.target.result;
        const objectStore = db.createObjectStore("cached", { keyPath: "code" });

        objectStore.transaction.oncomplete = (event) => {
            const customerObjectStore = db.transaction("cached", "readwrite").objectStore("cached");
            customerData.forEach((customer) => {
                customerObjectStore.add(customer);
            });
        };
    };

}

class CreateRequestLocalDb extends LocalDb {
    constructor() {
        LocalDb.apply(this, "Bards", 1);
    }
}