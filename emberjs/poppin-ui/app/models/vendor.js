import Model, { attr } from '@ember-data/model';

export default class LocationModel extends Model {
  @attr('string') organizationName;
  @attr('string') organizationContactName;
  @attr('string') organizationContactEmail;
	@attr('boolean') active;
	@attr('string') parentVendorId;
	@attr('date') lastUpdate;
	@attr adminIds;
	@attr memberIds;
	@attr subVendorIds;
	@attr locationIds;
	
	@attr admins;
	@attr members;
	@attr locations;
	@attr subVendors;
	@attr parent;
}
