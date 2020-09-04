import Model, { attr } from '@ember-data/model';

export default class LocationModel extends Model {
  @attr('string') id;
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
}
