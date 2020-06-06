import Service, { inject as injectService } from '@ember/service';
import { tracked } from '@glimmer/tracking';
import HttpResources from '../utils/http-resources';

export default class LocationsService extends Service {
	@injectService apiService;

	createNewLocation(location) {
		return this.apiService.request({
			resource: HttpResources.createNewLocation,
			body: location
		});
	}

	updateLocation(location) {
		return this.apiService.request({
			resource: HttpResources.updateLocation,
			body: location
		});
	}
}
