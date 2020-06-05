import Service, { inject as injectService } from '@ember/service';
import { tracked } from '@glimmer/tracking';
import HttpResources from '../utils/http-resources';

export default class LocationsService extends Service {
	@injectService apiService;
	@injectService store;

	createNewLocation(locationDTO) {
		return this.apiService.request({
			resource: HttpResources.createNewLocation,
			body: locationDTO
		}).then((location) => {
			this.store.createRecord('location', location);
			return this.apiService.resources.getYelpMatch({ locId: location.id });
		});
	}
}
