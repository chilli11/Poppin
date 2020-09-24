import ApplicationSerializer from './application';

export default class VendorSerializer extends ApplicationSerializer {
  serialize(snapshot, options) {
    let json = super.serialize(...arguments);

		delete json.members;
    delete json.admins;
		delete json.locations;
		delete json.subVendors;
		delete json.parent;

    return json;
	}
	
  normalizeResponse(store, primaryModelClass, payload, id, requestType) {		
		payload.id = payload.vendor.id;
		payload.parentVendorId = payload.vendor.parentVendorId;
		payload.organizationName = payload.vendor.organizationName;
		payload.organizationContactName = payload.vendor.organizationContactName;
		payload.organizationContactEmail = payload.vendor.organizationContactEmail;
		payload.adminIds = payload.vendor.adminIds;
		payload.memberIds = payload.vendor.memberIds;
		payload.subVendorIds = payload.vendor.subVendorIds;
		payload.locationIds = payload.vendor.locationIds;

    delete payload.vendor;

    return super.normalizeResponse(...arguments);
  }
}
