using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Devices {
    public class DriveProvider : IDisposable {
        #region Lifetime
        private static Lazy<DriveProvider> _instance = new Lazy<DriveProvider>(() => new DriveProvider());
        public static DriveProvider Current {
            get {
                return _instance.Value;
            }
        }
        private DriveProvider() {
            _queryLogicalDisk = new ManagementObjectSearcher(@"\root\cimv2", "SELECT * FROM WIN32_LogicalDisk");
            _queryPhysicalDisk = new ManagementObjectSearcher(@"\root\cimv2", "SELECT * FROM WIN32_DiskDrive");
            _queryPhysicalDiskToPartition = new ManagementObjectSearcher(@"\root\cimv2", "SELECT * FROM WIN32_DiskDriveToDiskPartition");
            _queryLogicalDiskToPartition = new ManagementObjectSearcher(@"\root\cimv2", "SELECT * FROM Win32_LogicalDiskToPartition");

        }
        private void _throwIfDisposed() {
            if (_isDisposed) throw new ObjectDisposedException(this.ToString());
        }
        private bool _isDisposed;
        public void Dispose() {
            if (_isDisposed) return;
            _queryLogicalDisk.Dispose();
            _queryLogicalDiskToPartition.Dispose();
            _queryPhysicalDisk.Dispose();
            _queryPhysicalDiskToPartition.Dispose();
            _isDisposed = true;
        }
        #endregion
        #region Queries
        private ManagementObjectSearcher _queryLogicalDiskToPartition;
        private ManagementObjectSearcher _queryPhysicalDiskToPartition;
        private ManagementObjectSearcher _queryPhysicalDisk;
        private ManagementObjectSearcher _queryLogicalDisk;


        public IEnumerable<ManagementPath> LogicalDisks {
            get {
                using (var objects = _queryLogicalDisk.Get())
                    foreach (var mo in objects) {
                        yield return (mo as ManagementObject)?.Path;
                        mo.Dispose();
                    }
            }
        }
        public IEnumerable<ManagementPath> PhysicalDisks {
            get {
                using (var objects = _queryPhysicalDisk.Get())
                    foreach (var mo in objects) {
                        yield return (mo as ManagementObject)?.Path;
                        mo.Dispose();
                    }
            }
        }

        public IEnumerable<ManagementPath> MapLogicalToPartition {
            get {
                using (var objects = _queryLogicalDiskToPartition.Get()) {
                    foreach (var mo in objects) {
                        yield return (mo as ManagementObject)?.Path;
                        mo.Dispose();
                    }
                }
            }
        }
        public IEnumerable<ManagementPath> MapPhysicalToPartition {
            get {
                using (var objects = _queryPhysicalDiskToPartition.Get()) {
                    foreach (var mo in objects) {
                        yield return (mo as ManagementObject).Path;
                        mo.Dispose();
                    }
                }
            }
        }
        public IEnumerable<IGrouping<ManagementPath, ManagementPath>> LogicalDisksByPhysicalDisk {
            get {

                var ql2p = from path in MapLogicalToPartition select new ManagementObject(path);
                var qp2p = from path in MapPhysicalToPartition select new ManagementObject(path);
                var disposeList = ql2p.Concat(qp2p);
                Console.WriteLine("Initializing Objects");
                 
                var ldbpd = (from l2p in ql2p
                             join p2p in qp2p on new ManagementPath(l2p["Antecedent"] as string).Path equals new ManagementPath(p2p["Dependent"] as string).Path
                             group new ManagementPath(l2p["Dependent"] as string) by new ManagementPath(p2p["Antecedent"] as string)).ToList();
                foreach (var item in disposeList) {
                    item.Dispose();
                }
                return ldbpd;

            }
        }
        #endregion

    }
}
